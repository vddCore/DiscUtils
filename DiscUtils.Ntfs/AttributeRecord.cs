using System;
using System.IO;
using System.Text;
using DiscUtils.Streams;
using DiscUtils.Streams.Buffer;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal abstract class AttributeRecord : IComparable<AttributeRecord>
    {
        protected ushort _attributeId;
        protected AttributeFlags _flags;

        protected string _name;
        protected byte _nonResidentFlag;
        protected AttributeType _type;

        public AttributeRecord() {}

        public AttributeRecord(AttributeType type, string name, ushort id, AttributeFlags flags)
        {
            _type = type;
            _name = name;
            _attributeId = id;
            _flags = flags;
        }

        public abstract long AllocatedLength { get; set; }

        public ushort AttributeId
        {
            get => _attributeId;
            set => _attributeId = value;
        }

        public AttributeType AttributeType => _type;

        public abstract long DataLength { get; set; }

        public AttributeFlags Flags
        {
            get => _flags;
            set => _flags = value;
        }

        public abstract long InitializedDataLength { get; set; }

        public bool IsNonResident => _nonResidentFlag != 0;

        public string Name => _name;

        public abstract int Size { get; }

        public abstract long StartVcn { get; }

        public int CompareTo(AttributeRecord other)
        {
            int val = (int)_type - (int)other._type;
            if (val != 0)
            {
                return val;
            }

            val = string.Compare(_name, other._name, StringComparison.OrdinalIgnoreCase);
            if (val != 0)
            {
                return val;
            }

            return _attributeId - other._attributeId;
        }

        public static AttributeRecord FromBytes(byte[] buffer, int offset, out int length)
        {
            if (EndianUtilities.ToUInt32LittleEndian(buffer, offset) == 0xFFFFFFFF)
            {
                length = 0;
                return null;
            }
            if (buffer[offset + 0x08] != 0x00)
            {
                return new NonResidentAttributeRecord(buffer, offset, out length);
            }
            return new ResidentAttributeRecord(buffer, offset, out length);
        }

        public static int CompareStartVcns(AttributeRecord x, AttributeRecord y)
        {
            if (x.StartVcn < y.StartVcn)
            {
                return -1;
            }
            if (x.StartVcn == y.StartVcn)
            {
                return 0;
            }
            return 1;
        }

        public abstract Range<long, long>[] GetClusters();

        public abstract IBuffer GetReadOnlyDataBuffer(INtfsContext context);

        public abstract int Write(byte[] buffer, int offset);

        public virtual void Dump(TextWriter writer, string indent)
        {
            writer.WriteLine(indent + "ATTRIBUTE RECORD");
            writer.WriteLine(indent + "            Type: " + _type);
            writer.WriteLine(indent + "    Non-Resident: " + _nonResidentFlag);
            writer.WriteLine(indent + "            Name: " + _name);
            writer.WriteLine(indent + "           Flags: " + _flags);
            writer.WriteLine(indent + "     AttributeId: " + _attributeId);
        }

        protected virtual void Read(byte[] buffer, int offset, out int length)
        {
            _type = (AttributeType)EndianUtilities.ToUInt32LittleEndian(buffer, offset + 0x00);
            length = EndianUtilities.ToInt32LittleEndian(buffer, offset + 0x04);

            _nonResidentFlag = buffer[offset + 0x08];
            byte nameLength = buffer[offset + 0x09];
            ushort nameOffset = EndianUtilities.ToUInt16LittleEndian(buffer, offset + 0x0A);
            _flags = (AttributeFlags)EndianUtilities.ToUInt16LittleEndian(buffer, offset + 0x0C);
            _attributeId = EndianUtilities.ToUInt16LittleEndian(buffer, offset + 0x0E);

            if (nameLength != 0x00)
            {
                if (nameLength + nameOffset > length)
                {
                    throw new IOException("Corrupt attribute, name outside of attribute");
                }

                _name = Encoding.Unicode.GetString(buffer, offset + nameOffset, nameLength * 2);
            }
        }
    }
}