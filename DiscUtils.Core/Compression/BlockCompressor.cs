namespace DiscUtils.Core.Compression
{
    /// <summary>
    /// Base class for block compression algorithms.
    /// </summary>
    public abstract class BlockCompressor
    {
        /// <summary>
        /// Gets or sets the block size parameter to the algorithm.
        /// </summary>
        /// <remarks>
        /// Some algorithms may use this to control both compression and decompression, others may
        /// only use it to control compression.  Some may ignore it entirely.
        /// </remarks>
        public int BlockSize { get; set; }

        /// <summary>
        /// Compresses some data.
        /// </summary>
        /// <param name="source">The uncompressed input.</param>
        /// <param name="sourceOffset">Offset of the input data in <c>source</c>.</param>
        /// <param name="sourceLength">The amount of uncompressed data.</param>
        /// <param name="compressed">The destination for the output compressed data.</param>
        /// <param name="compressedOffset">Offset for the output data in <c>compressed</c>.</param>
        /// <param name="compressedLength">The maximum size of the compressed data on input, and the actual size on output.</param>
        /// <returns>Indication of success, or indication the data could not compress into the requested space.</returns>
        public abstract CompressionResult Compress(byte[] source, int sourceOffset, int sourceLength, byte[] compressed,
                                                   int compressedOffset, ref int compressedLength);

        /// <summary>
        /// Decompresses some data.
        /// </summary>
        /// <param name="source">The compressed input.</param>
        /// <param name="sourceOffset">Offset of the input data in <c>source</c>.</param>
        /// <param name="sourceLength">The amount of compressed data.</param>
        /// <param name="decompressed">The destination for the output decompressed data.</param>
        /// <param name="decompressedOffset">Offset for the output data in <c>decompressed</c>.</param>
        /// <returns>The amount of decompressed data.</returns>
        public abstract int Decompress(byte[] source, int sourceOffset, int sourceLength, byte[] decompressed,
                                       int decompressedOffset);
    }
}