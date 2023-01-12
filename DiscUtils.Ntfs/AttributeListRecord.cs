using System;
using System.IO;
using System.Text;
using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal class AttributeListRecord : IDiagnosticTraceable, IByteArraySerializable, IComparable<AttributeListRecord>
    {
        public ushort AttributeId;
        public FileRecordReference BaseFileReference;
        public string Name;
        public byte NameLength;
        public byte NameOffset;
        public ushort RecordLength;
        public ulong StartVcn;
        public AttributeType Type;

        public int Size =>
            MathUtilities.RoundUp(0x20 + (string.IsNullOrEmpty(Name) ? 0 : Encoding.Unicode.GetByteCount(Name)),
                8);

        public int ReadFrom(byte[] data, int offset)
        {
            Type = (AttributeType)EndianUtilities.ToUInt32LittleEndian(data, offset + 0x00);
            RecordLength = EndianUtilities.ToUInt16LittleEndian(data, offset + 0x04);
            NameLength = data[offset + 0x06];
            NameOffset = data[offset + 0x07];
            StartVcn = EndianUtilities.ToUInt64LittleEndian(data, offset + 0x08);
            BaseFileReference = new FileRecordReference(EndianUtilities.ToUInt64LittleEndian(data, offset + 0x10));
            AttributeId = EndianUtilities.ToUInt16LittleEndian(data, offset + 0x18);

            if (NameLength > 0)
            {
                Name = Encoding.Unicode.GetString(data, offset + NameOffset, NameLength * 2);
            }
            else
            {
                Name = null;
            }

            if (RecordLength < 0x18)
            {
                throw new InvalidDataException("Malformed AttributeList record");
            }

            return RecordLength;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            NameOffset = 0x20;
            if (string.IsNullOrEmpty(Name))
            {
                NameLength = 0;
            }
            else
            {
                NameLength = (byte)(Encoding.Unicode.GetBytes(Name, 0, Name.Length, buffer, offset + NameOffset) / 2);
            }

            RecordLength = (ushort)MathUtilities.RoundUp(NameOffset + NameLength * 2, 8);

            EndianUtilities.WriteBytesLittleEndian((uint)Type, buffer, offset);
            EndianUtilities.WriteBytesLittleEndian(RecordLength, buffer, offset + 0x04);
            buffer[offset + 0x06] = NameLength;
            buffer[offset + 0x07] = NameOffset;
            EndianUtilities.WriteBytesLittleEndian(StartVcn, buffer, offset + 0x08);
            EndianUtilities.WriteBytesLittleEndian(BaseFileReference.Value, buffer, offset + 0x10);
            EndianUtilities.WriteBytesLittleEndian(AttributeId, buffer, offset + 0x18);
        }

        public int CompareTo(AttributeListRecord other)
        {
            int val = (int)Type - (int)other.Type;
            if (val != 0)
            {
                return val;
            }

            val = string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
            if (val != 0)
            {
                return val;
            }

            return (int)StartVcn - (int)other.StartVcn;
        }

        public void Dump(TextWriter writer, string indent)
        {
            writer.WriteLine(indent + "ATTRIBUTE LIST RECORD");
            writer.WriteLine(indent + "                 Type: " + Type);
            writer.WriteLine(indent + "        Record Length: " + RecordLength);
            writer.WriteLine(indent + "                 Name: " + Name);
            writer.WriteLine(indent + "            Start VCN: " + StartVcn);
            writer.WriteLine(indent + "  Base File Reference: " + BaseFileReference);
            writer.WriteLine(indent + "         Attribute ID: " + AttributeId);
        }

        public static AttributeListRecord FromAttribute(AttributeRecord attr, FileRecordReference mftRecord)
        {
            AttributeListRecord newRecord = new AttributeListRecord
            {
                Type = attr.AttributeType,
                Name = attr.Name,
                StartVcn = 0,
                BaseFileReference = mftRecord,
                AttributeId = attr.AttributeId
            };

            if (attr.IsNonResident)
            {
                newRecord.StartVcn = (ulong)((NonResidentAttributeRecord)attr).StartVcn;
            }

            return newRecord;
        }
    }
}