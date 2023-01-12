namespace DiscUtils.Core.Compression
{
    /// <summary>
    /// Base class for bit streams.
    /// </summary>
    /// <remarks>
    /// The rules for conversion of a byte stream to a bit stream vary
    /// between implementations.
    /// </remarks>
    internal abstract class BitStream
    {
        /// <summary>
        /// Gets the maximum number of bits that can be peeked on the stream.
        /// </summary>
        public abstract int MaxReadAhead { get; }

        /// <summary>
        /// Reads bits from the stream.
        /// </summary>
        /// <param name="count">The number of bits to read.</param>
        /// <returns>The bits as a UInt32.</returns>
        public abstract uint Read(int count);

        /// <summary>
        /// Queries data from the stream.
        /// </summary>
        /// <param name="count">The number of bits to query.</param>
        /// <returns>The bits as a UInt32.</returns>
        /// <remarks>This method does not consume the bits (i.e. move the file pointer).</remarks>
        public abstract uint Peek(int count);

        /// <summary>
        /// Consumes bits from the stream without returning them.
        /// </summary>
        /// <param name="count">The number of bits to consume.</param>
        public abstract void Consume(int count);
    }
}