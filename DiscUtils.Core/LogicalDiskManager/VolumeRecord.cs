using System;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.LogicalDiskManager
{
    internal sealed class VolumeRecord : DatabaseRecord
    {
        public string ActiveString;
        public byte BiosType;
        public ulong ComponentCount;
        public ulong DupCount; // ??Seen once after adding 'foreign disk', from broken mirror (identical links(P/V/C))
        public string GenString;
        public string MountHint;
        public string NumberString; // 8000000000000000 sometimes...
        public uint PartitionComponentLink;
        public long Size;
        public ulong Unknown1; // Zero
        public uint Unknown2; // Zero
        public ulong UnknownA; // Zero
        public ulong UnknownB; // 00 .. 03
        public uint UnknownC; // 00 00 00 11
        public uint UnknownD; // Zero
        public Guid VolumeGuid;

        protected override void DoReadFrom(byte[] buffer, int offset)
        {
            base.DoReadFrom(buffer, offset);

            int pos = offset + 0x18;

            Id = ReadVarULong(buffer, ref pos);
            Name = ReadVarString(buffer, ref pos);
            GenString = ReadVarString(buffer, ref pos);
            NumberString = ReadVarString(buffer, ref pos);
            ActiveString = ReadString(buffer, 6, ref pos);
            UnknownA = ReadVarULong(buffer, ref pos);
            UnknownB = ReadULong(buffer, ref pos);
            DupCount = ReadVarULong(buffer, ref pos);
            UnknownC = ReadUInt(buffer, ref pos);
            ComponentCount = ReadVarULong(buffer, ref pos);
            UnknownD = ReadUInt(buffer, ref pos);
            PartitionComponentLink = ReadUInt(buffer, ref pos);
            Unknown1 = ReadULong(buffer, ref pos);
            Size = ReadVarLong(buffer, ref pos);
            Unknown2 = ReadUInt(buffer, ref pos);
            BiosType = ReadByte(buffer, ref pos);
            VolumeGuid = EndianUtilities.ToGuidBigEndian(buffer, pos);
            pos += 16;

            if ((Flags & 0x0200) != 0)
            {
                MountHint = ReadVarString(buffer, ref pos);
            }
        }
    }
}