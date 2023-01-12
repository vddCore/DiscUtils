using System;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal struct FileRecordReference : IByteArraySerializable, IComparable<FileRecordReference>
    {
        public FileRecordReference(ulong val)
        {
            Value = val;
        }

        public FileRecordReference(long mftIndex, ushort sequenceNumber)
        {
            Value = (ulong)(mftIndex & 0x0000FFFFFFFFFFFFL) |
                    ((ulong)sequenceNumber << 48 & 0xFFFF000000000000L);
        }

        public ulong Value { get; private set; }

        public long MftIndex => (long)(Value & 0x0000FFFFFFFFFFFFL);

        public ushort SequenceNumber => (ushort)((Value >> 48) & 0xFFFF);

        public int Size => 8;

        public bool IsNull => SequenceNumber == 0;

        public static bool operator ==(FileRecordReference a, FileRecordReference b)
        {
            return a.Value == b.Value;
        }

        public static bool operator !=(FileRecordReference a, FileRecordReference b)
        {
            return a.Value != b.Value;
        }

        public int ReadFrom(byte[] buffer, int offset)
        {
            Value = EndianUtilities.ToUInt64LittleEndian(buffer, offset);
            return 8;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            EndianUtilities.WriteBytesLittleEndian(Value, buffer, offset);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is FileRecordReference))
            {
                return false;
            }

            return Value == ((FileRecordReference)obj).Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(FileRecordReference other)
        {
            if (Value < other.Value)
            {
                return -1;
            }
            if (Value > other.Value)
            {
                return 1;
            }
            return 0;
        }

        public override string ToString()
        {
            return "MFT:" + MftIndex + " (ver: " + SequenceNumber + ")";
        }
    }
}