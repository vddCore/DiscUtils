using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal class IndexHeader
    {
        public const int Size = 0x10;
        public uint AllocatedSizeOfEntries;
        public byte HasChildNodes;

        public uint OffsetToFirstEntry;
        public uint TotalSizeOfEntries;

        public IndexHeader(uint allocatedSize)
        {
            AllocatedSizeOfEntries = allocatedSize;
        }

        public IndexHeader(byte[] data, int offset)
        {
            OffsetToFirstEntry = EndianUtilities.ToUInt32LittleEndian(data, offset + 0x00);
            TotalSizeOfEntries = EndianUtilities.ToUInt32LittleEndian(data, offset + 0x04);
            AllocatedSizeOfEntries = EndianUtilities.ToUInt32LittleEndian(data, offset + 0x08);
            HasChildNodes = data[offset + 0x0C];
        }

        internal void WriteTo(byte[] buffer, int offset)
        {
            EndianUtilities.WriteBytesLittleEndian(OffsetToFirstEntry, buffer, offset + 0x00);
            EndianUtilities.WriteBytesLittleEndian(TotalSizeOfEntries, buffer, offset + 0x04);
            EndianUtilities.WriteBytesLittleEndian(AllocatedSizeOfEntries, buffer, offset + 0x08);
            buffer[offset + 0x0C] = HasChildNodes;
            buffer[offset + 0x0D] = 0;
            buffer[offset + 0x0E] = 0;
            buffer[offset + 0x0F] = 0;
        }
    }
}