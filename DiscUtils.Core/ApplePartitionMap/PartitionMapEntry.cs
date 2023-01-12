using System;
using System.IO;
using DiscUtils.Core.Partitions;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.ApplePartitionMap
{
    internal sealed class PartitionMapEntry : PartitionInfo, IByteArraySerializable
    {
        private readonly Stream _diskStream;
        public uint BootBlock;
        public uint BootBytes;
        public uint Flags;
        public uint LogicalBlocks;
        public uint LogicalBlockStart;
        public uint MapEntries;
        public string Name;
        public uint PhysicalBlocks;
        public uint PhysicalBlockStart;
        public ushort Signature;
        public string Type;

        public PartitionMapEntry(Stream diskStream)
        {
            _diskStream = diskStream;
        }

        public override byte BiosType => 0xAF;

        public override long FirstSector => PhysicalBlockStart;

        public override Guid GuidType => Guid.Empty;

        public override long LastSector => PhysicalBlockStart + PhysicalBlocks - 1;

        public override string TypeAsString => Type;

        internal override PhysicalVolumeType VolumeType => PhysicalVolumeType.ApplePartition;

        public int Size => 512;

        public int ReadFrom(byte[] buffer, int offset)
        {
            Signature = EndianUtilities.ToUInt16BigEndian(buffer, offset + 0);
            MapEntries = EndianUtilities.ToUInt32BigEndian(buffer, offset + 4);
            PhysicalBlockStart = EndianUtilities.ToUInt32BigEndian(buffer, offset + 8);
            PhysicalBlocks = EndianUtilities.ToUInt32BigEndian(buffer, offset + 12);
            Name = EndianUtilities.BytesToString(buffer, offset + 16, 32).TrimEnd('\0');
            Type = EndianUtilities.BytesToString(buffer, offset + 48, 32).TrimEnd('\0');
            LogicalBlockStart = EndianUtilities.ToUInt32BigEndian(buffer, offset + 80);
            LogicalBlocks = EndianUtilities.ToUInt32BigEndian(buffer, offset + 84);
            Flags = EndianUtilities.ToUInt32BigEndian(buffer, offset + 88);
            BootBlock = EndianUtilities.ToUInt32BigEndian(buffer, offset + 92);
            BootBytes = EndianUtilities.ToUInt32BigEndian(buffer, offset + 96);

            return 512;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }

        public override SparseStream Open()
        {
            return new SubStream(_diskStream, PhysicalBlockStart * 512, PhysicalBlocks * 512);
        }
    }
}