using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Streams.Util;

namespace DiscUtils.Streams
{
    public class StripedStream : SparseStream
    {
        private readonly bool _canRead;
        private readonly bool _canWrite;
        private readonly long _length;
        private readonly Ownership _ownsWrapped;

        private long _position;
        private readonly long _stripeSize;
        private List<SparseStream> _wrapped;

        public StripedStream(long stripeSize, Ownership ownsWrapped, params SparseStream[] wrapped)
        {
            _wrapped = new List<SparseStream>(wrapped);
            _stripeSize = stripeSize;
            _ownsWrapped = ownsWrapped;

            _canRead = _wrapped[0].CanRead;
            _canWrite = _wrapped[0].CanWrite;
            long subStreamLength = _wrapped[0].Length;

            foreach (SparseStream stream in _wrapped)
            {
                if (stream.CanRead != _canRead || stream.CanWrite != _canWrite)
                {
                    throw new ArgumentException("All striped streams must have the same read/write permissions",
                        nameof(wrapped));
                }

                if (stream.Length != subStreamLength)
                {
                    throw new ArgumentException("All striped streams must have the same length", nameof(wrapped));
                }
            }

            _length = subStreamLength * wrapped.Length;
        }

        public override bool CanRead => _canRead;

        public override bool CanSeek => true;

        public override bool CanWrite => _canWrite;

        public override IEnumerable<StreamExtent> Extents
        {
            get
            {
                // Temporary, indicate there are no 'unstored' extents.
                // Consider combining extent information from all wrapped streams in future.
                yield return new StreamExtent(0, _length);
            }
        }

        public override long Length => _length;

        public override long Position
        {
            get => _position;

            set => _position = value;
        }

        public override void Flush()
        {
            foreach (SparseStream stream in _wrapped)
            {
                stream.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
            {
                throw new InvalidOperationException("Attempt to read to non-readable stream");
            }

            int maxToRead = (int)Math.Min(_length - _position, count);

            int totalRead = 0;
            while (totalRead < maxToRead)
            {
                long stripe = _position / _stripeSize;
                long stripeOffset = _position % _stripeSize;
                int stripeToRead = (int)Math.Min(maxToRead - totalRead, _stripeSize - stripeOffset);

                int streamIdx = (int)(stripe % _wrapped.Count);
                long streamStripe = stripe / _wrapped.Count;

                Stream targetStream = _wrapped[streamIdx];
                targetStream.Position = streamStripe * _stripeSize + stripeOffset;

                int numRead = targetStream.Read(buffer, offset + totalRead, stripeToRead);
                _position += numRead;
                totalRead += numRead;
            }

            return totalRead;
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
                effectiveOffset += _length;
            }

            if (effectiveOffset < 0)
            {
                throw new IOException("Attempt to move before beginning of stream");
            }
            _position = effectiveOffset;
            return _position;
        }

        public override void SetLength(long value)
        {
            if (value != _length)
            {
                throw new InvalidOperationException("Changing the stream length is not permitted for striped streams");
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
            {
                throw new InvalidOperationException("Attempt to write to read-only stream");
            }

            if (_position + count > _length)
            {
                throw new IOException("Attempt to write beyond end of stream");
            }

            int totalWritten = 0;
            while (totalWritten < count)
            {
                long stripe = _position / _stripeSize;
                long stripeOffset = _position % _stripeSize;
                int stripeToWrite = (int)Math.Min(count - totalWritten, _stripeSize - stripeOffset);

                int streamIdx = (int)(stripe % _wrapped.Count);
                long streamStripe = stripe / _wrapped.Count;

                Stream targetStream = _wrapped[streamIdx];
                targetStream.Position = streamStripe * _stripeSize + stripeOffset;
                targetStream.Write(buffer, offset + totalWritten, stripeToWrite);

                _position += stripeToWrite;
                totalWritten += stripeToWrite;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && _ownsWrapped == Ownership.Dispose && _wrapped != null)
                {
                    foreach (SparseStream stream in _wrapped)
                    {
                        stream.Dispose();
                    }

                    _wrapped = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}