namespace DiscUtils.Core.LogicalDiskManager
{
    internal sealed class DiskGroupRecord : DatabaseRecord
    {
        public string GroupGuidString;
        public uint Unknown1;

        protected override void DoReadFrom(byte[] buffer, int offset)
        {
            base.DoReadFrom(buffer, offset);

            int pos = offset + 0x18;

            Id = ReadVarULong(buffer, ref pos);
            Name = ReadVarString(buffer, ref pos);
            if ((Flags & 0xF0) == 0x40)
            {
                GroupGuidString = ReadBinaryGuid(buffer, ref pos).ToString();
            }
            else
            {
                GroupGuidString = ReadVarString(buffer, ref pos);
            }
            Unknown1 = ReadUInt(buffer, ref pos);
        }
    }
}