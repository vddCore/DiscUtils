namespace DiscUtils.Core.LogicalDiskManager
{
    internal sealed class DiskRecord : DatabaseRecord
    {
        public string DiskGuidString;

        protected override void DoReadFrom(byte[] buffer, int offset)
        {
            base.DoReadFrom(buffer, offset);

            int pos = offset + 0x18;

            Id = ReadVarULong(buffer, ref pos);
            Name = ReadVarString(buffer, ref pos);
            if ((Flags & 0xF0) == 0x40)
            {
                DiskGuidString = ReadBinaryGuid(buffer, ref pos).ToString();
            }
            else
            {
                DiskGuidString = ReadVarString(buffer, ref pos);
            }
        }
    }
}