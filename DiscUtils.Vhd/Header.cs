using System.IO;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Vhd
{
    internal class Header
    {
        public string Cookie;
        public long DataOffset;

        public static Header FromStream(Stream stream)
        {
            return FromBytes(StreamUtilities.ReadExact(stream, 16), 0);
        }

        public static Header FromBytes(byte[] data, int offset)
        {
            Header result = new Header();
            result.Cookie = EndianUtilities.BytesToString(data, offset, 8);
            result.DataOffset = EndianUtilities.ToInt64BigEndian(data, offset + 8);
            return result;
        }
    }
}