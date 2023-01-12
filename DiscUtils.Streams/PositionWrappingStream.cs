using System;
using System.IO;
using DiscUtils.Streams.Util;

namespace DiscUtils.Streams
{
    /// <summary>
    /// Stream wrapper to allow forward only seeking on not seekable streams
    /// </summary>
    public class PositionWrappingStream : WrappingStream
    {
        public PositionWrappingStream(SparseStream toWrap, long currentPosition, Ownership ownership)
            : base(toWrap, ownership)
        {
            _position = currentPosition;
        }

        private long _position;
        public override long Position
        {
            get => _position;
            set
            {
                if (_position == value)
                    return;
                Seek(value, SeekOrigin.Begin);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (base.CanSeek)
            {
                return base.Seek(offset, SeekOrigin.Current);
            }
            switch (origin)
            {
                case SeekOrigin.Begin:
                    offset = offset - _position;
                    break;
                case SeekOrigin.Current:
                    offset = offset + _position;
                    break;
                case SeekOrigin.End:
                    offset = Length - offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }
            if (offset == 0)
                return _position;
            if (offset < 0)
                throw new NotSupportedException("backward seeking is not supported");
            var buffer = new byte[Sizes.OneKiB];
            while (offset > 0)
            {
                var read = base.Read(buffer, 0, (int)Math.Min(buffer.Length, offset));
                offset -= read;
            }
            return _position;
        }

        public override bool CanSeek => true;

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, count);
            _position += read;
            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            _position += count;
        }
    }
}