using DiscUtils.Streams.Util;

namespace DiscUtils.Vhd
{
    internal class ParentLocator
    {
        public const string PlatformCodeWindowsRelativeUnicode = "W2ru";
        public const string PlatformCodeWindowsAbsoluteUnicode = "W2ku";

        public string PlatformCode;
        public int PlatformDataLength;
        public long PlatformDataOffset;
        public int PlatformDataSpace;

        public ParentLocator()
        {
            PlatformCode = string.Empty;
        }

        public ParentLocator(ParentLocator toCopy)
        {
            PlatformCode = toCopy.PlatformCode;
            PlatformDataSpace = toCopy.PlatformDataSpace;
            PlatformDataLength = toCopy.PlatformDataLength;
            PlatformDataOffset = toCopy.PlatformDataOffset;
        }

        public static ParentLocator FromBytes(byte[] data, int offset)
        {
            ParentLocator result = new ParentLocator();
            result.PlatformCode = EndianUtilities.BytesToString(data, offset, 4);
            result.PlatformDataSpace = EndianUtilities.ToInt32BigEndian(data, offset + 4);
            result.PlatformDataLength = EndianUtilities.ToInt32BigEndian(data, offset + 8);
            result.PlatformDataOffset = EndianUtilities.ToInt64BigEndian(data, offset + 16);
            return result;
        }

        internal void ToBytes(byte[] data, int offset)
        {
            EndianUtilities.StringToBytes(PlatformCode, data, offset, 4);
            EndianUtilities.WriteBytesBigEndian(PlatformDataSpace, data, offset + 4);
            EndianUtilities.WriteBytesBigEndian(PlatformDataLength, data, offset + 8);
            EndianUtilities.WriteBytesBigEndian((uint)0, data, offset + 12);
            EndianUtilities.WriteBytesBigEndian(PlatformDataOffset, data, offset + 16);
        }
    }
}