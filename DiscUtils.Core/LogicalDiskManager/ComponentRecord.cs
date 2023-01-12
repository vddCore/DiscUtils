namespace DiscUtils.Core.LogicalDiskManager
{
    internal sealed class ComponentRecord : DatabaseRecord
    {
        public uint LinkId; // Identical on mirrors
        public ExtentMergeType MergeType; // (02 Spanned, Simple, Mirrored)  (01 on striped)
        public ulong NumExtents; // Could be num disks
        public string StatusString;
        public long StripeSizeSectors;
        public long StripeStride; // aka num partitions
        public uint Unknown1; // Zero
        public uint Unknown2; // Zero
        public ulong Unknown3; // 00 .. 00
        public ulong Unknown4; // ??
        public ulong VolumeId;

        protected override void DoReadFrom(byte[] buffer, int offset)
        {
            base.DoReadFrom(buffer, offset);

            int pos = offset + 0x18;

            Id = ReadVarULong(buffer, ref pos);
            Name = ReadVarString(buffer, ref pos);
            StatusString = ReadVarString(buffer, ref pos);
            MergeType = (ExtentMergeType)ReadByte(buffer, ref pos);
            Unknown1 = ReadUInt(buffer, ref pos); // Zero
            NumExtents = ReadVarULong(buffer, ref pos);
            Unknown2 = ReadUInt(buffer, ref pos);
            LinkId = ReadUInt(buffer, ref pos);
            Unknown3 = ReadULong(buffer, ref pos); // Zero
            VolumeId = ReadVarULong(buffer, ref pos);
            Unknown4 = ReadVarULong(buffer, ref pos); // Zero

            if ((Flags & 0x1000) != 0)
            {
                StripeSizeSectors = ReadVarLong(buffer, ref pos);
                StripeStride = ReadVarLong(buffer, ref pos);
            }
        }
    }
}