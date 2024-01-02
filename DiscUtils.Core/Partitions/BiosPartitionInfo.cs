using System;
using DiscUtils.Streams;

namespace DiscUtils.Core.Partitions
{
    /// <summary>
    /// Provides access to partition records in a BIOS (MBR) partition table.
    /// </summary>
    public sealed class BiosPartitionInfo : PartitionInfo
    {
        private readonly BiosPartitionRecord _record;
        private readonly BiosPartitionTable _table;

        internal BiosPartitionInfo(BiosPartitionTable table, BiosPartitionRecord record)
        {
            _table = table;
            _record = record;
        }

        /// <summary>
        /// Gets the type of the partition.
        /// </summary>
        public override byte BiosType => _record.PartitionType;

        /// <summary>
        /// Gets the end (inclusive) of the partition as a CHS address.
        /// </summary>
        public ChsAddress End => new ChsAddress(_record.EndCylinder, _record.EndHead, _record.EndSector);

        /// <summary>
        /// Gets the first sector of the partion (relative to start of disk) as a Logical Block Address.
        /// </summary>
        public override long FirstSector => _record.LBAStartAbsolute;

        /// <summary>
        /// Always returns <see cref="Guid"/>.Empty.
        /// </summary>
        public override Guid GuidType => Guid.Empty;

        /// <summary>
        /// Gets a value indicating whether this partition is active (bootable).
        /// </summary>
        public bool IsActive => _record.Status != 0;

        /// <summary>
        /// Gets a value indicating whether the partition is a primary (rather than extended) partition.
        /// </summary>
        public bool IsPrimary => PrimaryIndex >= 0;

        /// <summary>
        /// Gets the last sector of the partion (relative to start of disk) as a Logical Block Address (inclusive).
        /// </summary>
        public override long LastSector => _record.LBAStartAbsolute + _record.LBALength - 1;

        /// <summary>
        /// Gets the index of the partition in the primary partition table, or <c>-1</c> if not a primary partition.
        /// </summary>
        public int PrimaryIndex => _record.Index;

        /// <summary>
        /// Gets the start of the partition as a CHS address.
        /// </summary>
        public ChsAddress Start => new ChsAddress(_record.StartCylinder, _record.StartHead, _record.StartSector);

        /// <summary>
        /// Gets the type of the partition as a string.
        /// </summary>
        public override string TypeAsString => _record.FriendlyPartitionType;

        internal override PhysicalVolumeType VolumeType => PhysicalVolumeType.BiosPartition;

        /// <summary>
        /// Opens a stream to access the content of the partition.
        /// </summary>
        /// <returns>The new stream.</returns>
        public override SparseStream Open()
        {
            return _table.Open(_record);
        }
    }
}