using System;

namespace DiscUtils.Streams.Util
{
    /// <summary>
    /// Helper to count the number of bits set in a byte or byte[]
    /// </summary>
    public static class BitCounter
    {
        private static readonly byte[] _lookupTable;

        static BitCounter()
        {
            _lookupTable = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                byte bitCount = 0;
                var value = i;
                while (value != 0)
                {
                    bitCount++;
                    value &= (byte)(value - 1);
                }
                _lookupTable[i] = bitCount;
            }
        }

        /// <summary>
        /// count the number of bits set in <paramref name="value"/>
        /// </summary>
        /// <returns>the number of bits set in <paramref name="value"/></returns>
        public static byte Count(byte value)
        {
            return _lookupTable[value];
        }

        /// <summary>
        /// count the number of bits set in each entry of <paramref name="values"/>
        /// </summary>
        /// <param name="values">the <see cref="Array"/> to process</param>
        /// <param name="offset">the values offset to start from</param>
        /// <param name="count">the number of bytes to count</param>
        /// <returns></returns>
        public static long Count(byte[] values, int offset, int count)
        {
            var end = offset + count;
            if (end > values.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "can't count after end of values");
            var result = 0L;
            for (int i = offset; i < end; i++)
            {
                var value = values[i];
                result += _lookupTable[value];
            }
            return result;
        }
    }
}