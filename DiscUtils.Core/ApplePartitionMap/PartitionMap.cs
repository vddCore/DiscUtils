using System;
using System.Collections.ObjectModel;
using System.IO;
using DiscUtils.Core.Partitions;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.ApplePartitionMap
{
    /// <summary>
    /// Interprets Apple Partition Map structures that partition a disk.
    /// </summary>
    public sealed class PartitionMap : PartitionTable
    {
        private readonly PartitionMapEntry[] _partitions;
        private readonly Stream _stream;

        /// <summary>
        /// Initializes a new instance of the PartitionMap class.
        /// </summary>
        /// <param name="stream">Stream containing the contents of a disk.</param>
        public PartitionMap(Stream stream)
        {
            _stream = stream;

            stream.Position = 0;
            byte[] initialBytes = StreamUtilities.ReadExact(stream, 1024);

            BlockZero b0 = new BlockZero();
            b0.ReadFrom(initialBytes, 0);

            PartitionMapEntry initialPart = new PartitionMapEntry(_stream);
            initialPart.ReadFrom(initialBytes, 512);

            byte[] partTableData = StreamUtilities.ReadExact(stream, (int)(initialPart.MapEntries - 1) * 512);

            _partitions = new PartitionMapEntry[initialPart.MapEntries - 1];
            for (uint i = 0; i < initialPart.MapEntries - 1; ++i)
            {
                _partitions[i] = new PartitionMapEntry(_stream);
                _partitions[i].ReadFrom(partTableData, (int)(512 * i));
            }
        }

        /// <summary>
        /// Gets the GUID of the disk, always returns Guid.Empty.
        /// </summary>
        public override Guid DiskGuid => Guid.Empty;

        /// <summary>
        /// Gets the partitions present on the disk.
        /// </summary>
        public override ReadOnlyCollection<PartitionInfo> Partitions => new ReadOnlyCollection<PartitionInfo>(_partitions);

        /// <summary>
        /// Creates a new partition that encompasses the entire disk.
        /// </summary>
        /// <param name="type">The partition type.</param>
        /// <param name="active">Whether the partition is active (bootable).</param>
        /// <returns>The index of the partition.</returns>
        /// <remarks>The partition table must be empty before this method is called,
        /// otherwise IOException is thrown.</remarks>
        public override int Create(WellKnownPartitionType type, bool active)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new partition with a target size.
        /// </summary>
        /// <param name="size">The target size (in bytes).</param>
        /// <param name="type">The partition type.</param>
        /// <param name="active">Whether the partition is active (bootable).</param>
        /// <returns>The index of the new partition.</returns>
        public override int Create(long size, WellKnownPartitionType type, bool active)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new aligned partition that encompasses the entire disk.
        /// </summary>
        /// <param name="type">The partition type.</param>
        /// <param name="active">Whether the partition is active (bootable).</param>
        /// <param name="alignment">The alignment (in byte).</param>
        /// <returns>The index of the partition.</returns>
        /// <remarks>The partition table must be empty before this method is called,
        /// otherwise IOException is thrown.</remarks>
        /// <remarks>
        /// Traditionally partitions were aligned to the physical structure of the underlying disk,
        /// however with modern storage greater efficiency is acheived by aligning partitions on
        /// large values that are a power of two.
        /// </remarks>
        public override int CreateAligned(WellKnownPartitionType type, bool active, int alignment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new aligned partition with a target size.
        /// </summary>
        /// <param name="size">The target size (in bytes).</param>
        /// <param name="type">The partition type.</param>
        /// <param name="active">Whether the partition is active (bootable).</param>
        /// <param name="alignment">The alignment (in byte).</param>
        /// <returns>The index of the new partition.</returns>
        /// <remarks>
        /// Traditionally partitions were aligned to the physical structure of the underlying disk,
        /// however with modern storage greater efficiency is achieved by aligning partitions on
        /// large values that are a power of two.
        /// </remarks>
        public override int CreateAligned(long size, WellKnownPartitionType type, bool active, int alignment)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a partition at a given index.
        /// </summary>
        /// <param name="index">The index of the partition.</param>
        public override void Delete(int index)
        {
            throw new NotImplementedException();
        }
    }
}