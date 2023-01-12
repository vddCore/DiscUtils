using DiscUtils.Streams.Util;

namespace DiscUtils.Streams
{
    /// <summary>
    /// Represents a stream with a specified length
    /// </summary>
    /// <remarks>
    /// since the wrapped stream may not support <see cref="System.IO.Stream.Position"/>
    /// there is  no validation of the specified length
    /// </remarks>
    public class LengthWrappingStream : WrappingStream
    {
        private readonly long _length;

        public LengthWrappingStream(SparseStream toWrap, long length, Ownership ownership)
            : base(toWrap, ownership)
        {
            _length = length;
        }

        public override long Length => _length;
    }
}