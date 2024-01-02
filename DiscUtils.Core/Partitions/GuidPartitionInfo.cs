using System;
using DiscUtils.Streams;

namespace DiscUtils.Core.Partitions
{
    /// <summary>
    /// Provides access to partition records in a GUID partition table.
    /// </summary>
    public sealed class GuidPartitionInfo : PartitionInfo
    {
        private readonly GptEntry _entry;
        private readonly GuidPartitionTable _table;

        internal GuidPartitionInfo(GuidPartitionTable table, GptEntry entry)
        {
            _table = table;
            _entry = entry;
        }

        /// <summary>
        /// Gets the attributes of the partition.
        /// </summary>
        public long Attributes => (long)_entry.Attributes;

        /// <summary>
        /// Always returns Zero.
        /// </summary>
        public override byte BiosType => 0;

        /// <summary>
        /// Gets the first sector of the partion (relative to start of disk) as a Logical Block Address.
        /// </summary>
        public override long FirstSector => _entry.FirstUsedLogicalBlock;

        /// <summary>
        /// Gets the type of the partition, as a GUID.
        /// </summary>
        public override Guid GuidType => _entry.PartitionType;

        /// <summary>
        /// Gets the unique identity of this specific partition.
        /// </summary>
        public Guid Identity => _entry.Identity;

        /// <summary>
        /// Gets the last sector of the partion (relative to start of disk) as a Logical Block Address (inclusive).
        /// </summary>
        public override long LastSector => _entry.LastUsedLogicalBlock;

        /// <summary>
        /// Gets the name of the partition.
        /// </summary>
        public string Name => _entry.Name;

        /// <summary>
        /// Gets the type of the partition as a string.
        /// </summary>
        public override string TypeAsString => _entry.FriendlyPartitionType;

        internal override PhysicalVolumeType VolumeType => PhysicalVolumeType.GptPartition;

        /// <summary>
        /// Opens a stream to access the content of the partition.
        /// </summary>
        /// <returns>The new stream.</returns>
        public override SparseStream Open()
        {
            return _table.Open(_entry);
        }
    }
}