using System;
using System.IO;
using DiscUtils.Streams.Util;

namespace DiscUtils.Streams
{
    /// <summary>
    /// Aligns I/O to a given block size.
    /// </summary>
    /// <remarks>Uses the read-modify-write pattern to align I/O.</remarks>
    public sealed class AligningStream : WrappingMappedStream<SparseStream>
    {
        private readonly byte[] _alignmentBuffer;
        private readonly int _blockSize;
        private long _position;

        public AligningStream(SparseStream toWrap, Ownership ownership, int blockSize)
            : base(toWrap, ownership, null)
        {
            _blockSize = blockSize;
            _alignmentBuffer = new byte[blockSize];
        }

        public override long Position
        {
            get => _position;
            set => _position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int startOffset = (int)(_position % _blockSize);
            if (startOffset == 0 && (count % _blockSize == 0 || _position + count == Length))
            {
                // Aligned read - pass through to underlying stream.
                WrappedStream.Position = _position;
                int numRead = WrappedStream.Read(buffer, offset, count);
                _position += numRead;
                return numRead;
            }

            long startPos = MathUtilities.RoundDown(_position, _blockSize);
            long endPos = MathUtilities.RoundUp(_position + count, _blockSize);

            if (endPos - startPos > int.MaxValue)
            {
                throw new IOException("Oversized read, after alignment");
            }

            byte[] tempBuffer = new byte[endPos - startPos];

            WrappedStream.Position = startPos;
            int read = WrappedStream.Read(tempBuffer, 0, tempBuffer.Length);
            int available = Math.Min(count, read - startOffset);

            Array.Copy(tempBuffer, startOffset, buffer, offset, available);

            _position += available;
            return available;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long effectiveOffset = offset;
            if (origin == SeekOrigin.Current)
            {
                effectiveOffset += _position;
            }
            else if (origin == SeekOrigin.End)
            {
                effectiveOffset += Length;
            }

            if (effectiveOffset < 0)
            {
                throw new IOException("Attempt to move before beginning of stream");
            }
            _position = effectiveOffset;
            return _position;
        }

        public override void Clear(int count)
        {
            DoOperation(
                (s, opOffset, opCount) => { s.Clear(opCount); },
                (buffer, offset, opOffset, opCount) => { Array.Clear(buffer, offset, opCount); },
                count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            DoOperation(
                (s, opOffset, opCount) => { s.Write(buffer, offset + opOffset, opCount); },
                (tempBuffer, tempOffset, opOffset, opCount) => { Array.Copy(buffer, offset + opOffset, tempBuffer, tempOffset, opCount); },
                count);
        }

        private void DoOperation(ModifyStream modifyStream, ModifyBuffer modifyBuffer, int count)
        {
            int startOffset = (int)(_position % _blockSize);
            if (startOffset == 0 && (count % _blockSize == 0 || _position + count == Length))
            {
                WrappedStream.Position = _position;
                modifyStream(WrappedStream, 0, count);
                _position += count;
                return;
            }

            long unalignedEnd = _position + count;
            long alignedPos = MathUtilities.RoundDown(_position, _blockSize);

            if (startOffset != 0)
            {
                WrappedStream.Position = alignedPos;
                WrappedStream.Read(_alignmentBuffer, 0, _blockSize);

                modifyBuffer(_alignmentBuffer, startOffset, 0, Math.Min(count, _blockSize - startOffset));

                WrappedStream.Position = alignedPos;
                WrappedStream.Write(_alignmentBuffer, 0, _blockSize);
            }

            alignedPos = MathUtilities.RoundUp(_position, _blockSize);
            if (alignedPos >= unalignedEnd)
            {
                _position = unalignedEnd;
                return;
            }

            int passthroughLength = (int)MathUtilities.RoundDown(_position + count - alignedPos, _blockSize);
            if (passthroughLength > 0)
            {
                WrappedStream.Position = alignedPos;
                modifyStream(WrappedStream, (int)(alignedPos - _position), passthroughLength);
            }

            alignedPos += passthroughLength;
            if (alignedPos >= unalignedEnd)
            {
                _position = unalignedEnd;
                return;
            }

            WrappedStream.Position = alignedPos;
            WrappedStream.Read(_alignmentBuffer, 0, _blockSize);

            modifyBuffer(_alignmentBuffer, 0, (int)(alignedPos - _position), (int)Math.Min(count - (alignedPos - _position), unalignedEnd - alignedPos));

            WrappedStream.Position = alignedPos;
            WrappedStream.Write(_alignmentBuffer, 0, _blockSize);

            _position = unalignedEnd;
        }

        private delegate void ModifyStream(SparseStream stream, int opOffset, int count);

        private delegate void ModifyBuffer(byte[] buffer, int offset, int opOffset, int count);
    }
}