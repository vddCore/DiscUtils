using System;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class SecurityDescriptorRecord : IByteArraySerializable
    {
        public uint EntrySize;
        public uint Hash;
        public uint Id;
        public long OffsetInFile;
        public byte[] SecurityDescriptor;

        public int Size => SecurityDescriptor.Length + 0x14;

        public int ReadFrom(byte[] buffer, int offset)
        {
            Read(buffer, offset);
            return SecurityDescriptor.Length + 0x14;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            EntrySize = (uint)Size;

            EndianUtilities.WriteBytesLittleEndian(Hash, buffer, offset + 0x00);
            EndianUtilities.WriteBytesLittleEndian(Id, buffer, offset + 0x04);
            EndianUtilities.WriteBytesLittleEndian(OffsetInFile, buffer, offset + 0x08);
            EndianUtilities.WriteBytesLittleEndian(EntrySize, buffer, offset + 0x10);

            Array.Copy(SecurityDescriptor, 0, buffer, offset + 0x14, SecurityDescriptor.Length);
        }

        public bool Read(byte[] buffer, int offset)
        {
            Hash = EndianUtilities.ToUInt32LittleEndian(buffer, offset + 0x00);
            Id = EndianUtilities.ToUInt32LittleEndian(buffer, offset + 0x04);
            OffsetInFile = EndianUtilities.ToInt64LittleEndian(buffer, offset + 0x08);
            EntrySize = EndianUtilities.ToUInt32LittleEndian(buffer, offset + 0x10);

            if (EntrySize > 0)
            {
                SecurityDescriptor = new byte[EntrySize - 0x14];
                Array.Copy(buffer, offset + 0x14, SecurityDescriptor, 0, SecurityDescriptor.Length);
                return true;
            }
            return false;
        }
    }
}