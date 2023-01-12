using System;
using System.Collections.Generic;

namespace DiscUtils.Streams.Util
{
    /// <summary>
    /// Represents a range of values.
    /// </summary>
    /// <typeparam name="TOffset">The type of the offset element.</typeparam>
    /// <typeparam name="TCount">The type of the size element.</typeparam>
    public class Range<TOffset, TCount> : IEquatable<Range<TOffset, TCount>>
        where TOffset : IEquatable<TOffset>
        where TCount : IEquatable<TCount>
    {
        /// <summary>
        /// Initializes a new instance of the Range class.
        /// </summary>
        /// <param name="offset">The offset (i.e. start) of the range.</param>
        /// <param name="count">The size of the range.</param>
        public Range(TOffset offset, TCount count)
        {
            Offset = offset;
            Count = count;
        }

        /// <summary>
        /// Gets the size of the range.
        /// </summary>
        public TCount Count { get; }

        /// <summary>
        /// Gets the offset (i.e. start) of the range.
        /// </summary>
        public TOffset Offset { get; }

        #region IEquatable<Range<TOffset,TCount>> Members

        /// <summary>
        /// Compares this range to another.
        /// </summary>
        /// <param name="other">The range to compare.</param>
        /// <returns><c>true</c> if the ranges are equivalent, else <c>false</c>.</returns>
        public bool Equals(Range<TOffset, TCount> other)
        {
            if (other == null)
            {
                return false;
            }

            return Offset.Equals(other.Offset) && Count.Equals(other.Count);
        }

        #endregion

        /// <summary>
        /// Merges sets of ranges into chunks.
        /// </summary>
        /// <param name="ranges">The ranges to merge.</param>
        /// <param name="chunkSize">The size of each chunk.</param>
        /// <returns>Ranges combined into larger chunks.</returns>
        /// <typeparam name="T">The type of the offset and count in the ranges.</typeparam>
        public static IEnumerable<Range<T, T>> Chunked<T>(IEnumerable<Range<T, T>> ranges, T chunkSize)
            where T : struct, IEquatable<T>, IComparable<T>
        {
            T? chunkStart = Numbers<T>.Zero;
            T chunkLength = Numbers<T>.Zero;

            foreach (Range<T, T> range in ranges)
            {
                if (Numbers<T>.NotEqual(range.Count, Numbers<T>.Zero))
                {
                    T rangeStart = Numbers<T>.RoundDown(range.Offset, chunkSize);
                    T rangeNext = Numbers<T>.RoundUp(Numbers<T>.Add(range.Offset, range.Count), chunkSize);

                    if (chunkStart.HasValue &&
                        Numbers<T>.GreaterThan(rangeStart, Numbers<T>.Add(chunkStart.Value, chunkLength)))
                    {
                        // This extent is non-contiguous (in terms of blocks), so write out the last range and start new
                        yield return new Range<T, T>(chunkStart.Value, chunkLength);
                        chunkStart = rangeStart;
                    }
                    else if (!chunkStart.HasValue)
                    {
                        // First extent, so start first range
                        chunkStart = rangeStart;
                    }

                    // Set the length of the current range, based on the end of this extent
                    chunkLength = Numbers<T>.Subtract(rangeNext, chunkStart.Value);
                }
            }

            // Final range (if any ranges at all) hasn't been returned yet, so do that now
            if (chunkStart.HasValue)
            {
                yield return new Range<T, T>(chunkStart.Value, chunkLength);
            }
        }

        /// <summary>
        /// Returns a string representation of the extent as [start:+length].
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return "[" + Offset + ":+" + Count + "]";
        }
    }
}