using System;
using DiscUtils.Streams.Util;

namespace DiscUtils.Streams
{
    /// <summary>
    /// Represents a stream that is circular, so reads and writes off the end of the stream wrap.
    /// </summary>
    public sealed class CircularStream : WrappingStream
    {
        public CircularStream(SparseStream toWrap, Ownership ownership)
            : base(toWrap, ownership) {}

        public override int Read(byte[] buffer, int offset, int count)
        {
            WrapPosition();

            int read = base.Read(buffer, offset, (int)Math.Min(Length - Position, count));

            WrapPosition();

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WrapPosition();

            int totalWritten = 0;
            while (totalWritten < count)
            {
                int toWrite = (int)Math.Min(count - totalWritten, Length - Position);

                base.Write(buffer, offset + totalWritten, toWrite);

                WrapPosition();

                totalWritten += toWrite;
            }
        }

        private void WrapPosition()
        {
            long pos = Position;
            long length = Length;

            if (pos >= length)
            {
                Position = pos % length;
            }
        }
    }
}