using System;

namespace DiscUtils.Core.Compression
{
    /// <summary>
    /// Implementation of the Adler-32 checksum algorithm.
    /// </summary>
    public class Adler32
    {
        private uint _a;
        private uint _b;

        /// <summary>
        /// Initializes a new instance of the Adler32 class.
        /// </summary>
        public Adler32()
        {
            _a = 1;
        }

        /// <summary>
        /// Gets the checksum of all data processed so far.
        /// </summary>
        public int Value => (int)(_b << 16 | _a);

        /// <summary>
        /// Provides data that should be checksummed.
        /// </summary>
        /// <param name="buffer">Buffer containing the data to checksum.</param>
        /// <param name="offset">Offset of the first byte to checksum.</param>
        /// <param name="count">The number of bytes to checksum.</param>
        /// <remarks>
        /// Call this method repeatedly until all checksummed
        /// data has been processed.
        /// </remarks>
        public void Process(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0 || offset > buffer.Length)
            {
                throw new ArgumentException("Offset outside of array bounds", nameof(offset));
            }

            if (count < 0 || offset + count > buffer.Length)
            {
                throw new ArgumentException("Array index out of bounds", nameof(count));
            }

            int processed = 0;
            while (processed < count)
            {
                int innerEnd = Math.Min(count, processed + 2000);
                while (processed < innerEnd)
                {
                    _a += buffer[processed++];
                    _b += _a;
                }

                _a %= 65521;
                _b %= 65521;
            }
        }
    }
}