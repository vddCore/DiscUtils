namespace DiscUtils.Core.LogicalDiskManager
{
    internal sealed class ExtentRecord : DatabaseRecord
    {
        public ulong ComponentId;
        public ulong DiskId;
        public long DiskOffsetLba;
        public ulong InterleaveOrder;
        public long OffsetInVolumeLba;
        public uint PartitionComponentLink;
        public long SizeLba;
        public uint Unknown1;
        public uint Unknown2;

        protected override void DoReadFrom(byte[] buffer, int offset)
        {
            base.DoReadFrom(buffer, offset);

            int pos = offset + 0x18;

            Id = ReadVarULong(buffer, ref pos);
            Name = ReadVarString(buffer, ref pos);
            Unknown1 = ReadUInt(buffer, ref pos);
            Unknown2 = ReadUInt(buffer, ref pos);
            PartitionComponentLink = ReadUInt(buffer, ref pos);
            DiskOffsetLba = ReadLong(buffer, ref pos);
            OffsetInVolumeLba = ReadLong(buffer, ref pos);
            SizeLba = ReadVarLong(buffer, ref pos);
            ComponentId = ReadVarULong(buffer, ref pos);
            DiskId = ReadVarULong(buffer, ref pos);

            if ((Flags & 0x0800) != 0)
            {
                InterleaveOrder = ReadVarULong(buffer, ref pos);
            }
        }
    }
}