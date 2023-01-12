namespace DiscUtils.Streams.Block
{
    /// <summary>
    /// Statistical information about the effectiveness of a BlockCache instance.
    /// </summary>
    public sealed class BlockCacheStatistics
    {
        /// <summary>
        /// Gets the number of free blocks in the read cache.
        /// </summary>
        public int FreeReadBlocks { get; internal set; }

        /// <summary>
        /// Gets the number of requested 'large' reads, as defined by the LargeReadSize setting.
        /// </summary>
        public long LargeReadsIn { get; internal set; }

        /// <summary>
        /// Gets the number of times a read request was serviced (in part or whole) from the cache.
        /// </summary>
        public long ReadCacheHits { get; internal set; }

        /// <summary>
        /// Gets the number of time a read request was serviced (in part or whole) from the wrapped stream.
        /// </summary>
        public long ReadCacheMisses { get; internal set; }

        /// <summary>
        /// Gets the total number of requested reads.
        /// </summary>
        public long TotalReadsIn { get; internal set; }

        /// <summary>
        /// Gets the total number of reads passed on by the cache.
        /// </summary>
        public long TotalReadsOut { get; internal set; }

        /// <summary>
        /// Gets the total number of requested writes.
        /// </summary>
        public long TotalWritesIn { get; internal set; }

        /// <summary>
        /// Gets the number of requested unaligned reads.
        /// </summary>
        /// <remarks>Unaligned reads are reads where the read doesn't start on a multiple of
        /// the block size.</remarks>
        public long UnalignedReadsIn { get; internal set; }

        /// <summary>
        /// Gets the number of requested unaligned writes.
        /// </summary>
        /// <remarks>Unaligned writes are writes where the write doesn't start on a multiple of
        /// the block size.</remarks>
        public long UnalignedWritesIn { get; internal set; }
    }
}