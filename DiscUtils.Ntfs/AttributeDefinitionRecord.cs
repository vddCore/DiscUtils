using System.Text;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class AttributeDefinitionRecord
    {
        public const int Size = 0xA0;
        public AttributeCollationRule CollationRule;
        public uint DisplayRule;
        public AttributeTypeFlags Flags;
        public long MaxSize;
        public long MinSize;

        public string Name;
        public AttributeType Type;

        internal void Read(byte[] buffer, int offset)
        {
            Name = Encoding.Unicode.GetString(buffer, offset + 0, 128).Trim('\0');
            Type = (AttributeType)EndianUtilities.ToUInt32LittleEndian(buffer, offset + 0x80);
            DisplayRule = EndianUtilities.ToUInt32LittleEndian(buffer, offset + 0x84);
            CollationRule = (AttributeCollationRule)EndianUtilities.ToUInt32LittleEndian(buffer, offset + 0x88);
            Flags = (AttributeTypeFlags)EndianUtilities.ToUInt32LittleEndian(buffer, offset + 0x8C);
            MinSize = EndianUtilities.ToInt64LittleEndian(buffer, offset + 0x90);
            MaxSize = EndianUtilities.ToInt64LittleEndian(buffer, offset + 0x98);
        }

        internal void Write(byte[] buffer, int offset)
        {
            Encoding.Unicode.GetBytes(Name, 0, Name.Length, buffer, offset + 0);
            EndianUtilities.WriteBytesLittleEndian((uint)Type, buffer, offset + 0x80);
            EndianUtilities.WriteBytesLittleEndian(DisplayRule, buffer, offset + 0x84);
            EndianUtilities.WriteBytesLittleEndian((uint)CollationRule, buffer, offset + 0x88);
            EndianUtilities.WriteBytesLittleEndian((uint)Flags, buffer, offset + 0x8C);
            EndianUtilities.WriteBytesLittleEndian(MinSize, buffer, offset + 0x90);
            EndianUtilities.WriteBytesLittleEndian(MaxSize, buffer, offset + 0x98);
        }
    }
}