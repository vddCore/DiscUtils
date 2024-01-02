using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DiscUtils.Core.CoreCompat;
using DiscUtils.Core.Raw;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.Partitions
{
    /// <summary>
    /// Base class for classes which represent a disk partitioning scheme.
    /// </summary>
    /// <remarks>After modifying the table, by creating or deleting a partition assume that any
    /// previously stored partition indexes of higher value are no longer valid.  Re-enumerate
    /// the partitions to discover the next index-to-partition mapping.</remarks>
    public abstract class PartitionTable
    {
        private static List<PartitionTableFactory> _factories;

        /// <summary>
        /// Gets the number of User partitions on the disk.
        /// </summary>
        public int Count => Partitions.Count;

        /// <summary>
        /// Gets the GUID that uniquely identifies this disk, if supported (else returns <c>null</c>).
        /// </summary>
        public abstract Guid DiskGuid { get; }

        private static List<PartitionTableFactory> Factories
        {
            get
            {
                if (_factories == null)
                {
                    List<PartitionTableFactory> factories = new List<PartitionTableFactory>();

                    foreach (Type type in ReflectionHelper.GetAssembly(typeof(VolumeManager)).GetTypes())
                    {
                        foreach (PartitionTableFactoryAttribute attr in ReflectionHelper.GetCustomAttributes(type, typeof(PartitionTableFactoryAttribute), false))
                        {
                            factories.Add((PartitionTableFactory)Activator.CreateInstance(type));
                        }
                    }

                    _factories = factories;
                }

                return _factories;
            }
        }

        /// <summary>
        /// Gets information about a particular User partition.
        /// </summary>
        /// <param name="index">The index of the partition.</param>
        /// <returns>Information about the partition.</returns>
        public PartitionInfo this[int index] => Partitions[index];

        /// <summary>
        /// Gets the list of partitions that contain user data (i.e. non-system / empty).
        /// </summary>
        public abstract ReadOnlyCollection<PartitionInfo> Partitions { get; }

        /// <summary>
        /// Determines if a disk is partitioned with a known partitioning scheme.
        /// </summary>
        /// <param name="content">The content of the disk to check.</param>
        /// <returns><c>true</c> if the disk is partitioned, else <c>false</c>.</returns>
        public static bool IsPartitioned(Stream content)
        {
            foreach (PartitionTableFactory partTableFactory in Factories)
            {
                if (partTableFactory.DetectIsPartitioned(content))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a disk is partitioned with a known partitioning scheme.
        /// </summary>
        /// <param name="disk">The disk to check.</param>
        /// <returns><c>true</c> if the disk is partitioned, else <c>false</c>.</returns>
        public static bool IsPartitioned(VirtualDisk disk)
        {
            return IsPartitioned(disk.Content);
        }

        /// <summary>
        /// Gets all of the partition tables found on a disk.
        /// </summary>
        /// <param name="disk">The disk to inspect.</param>
        /// <returns>It is rare for a disk to have multiple partition tables, but theoretically
        /// possible.</returns>
        public static IList<PartitionTable> GetPartitionTables(VirtualDisk disk)
        {
            List<PartitionTable> tables = new List<PartitionTable>();

            foreach (PartitionTableFactory factory in Factories)
            {
                PartitionTable table = factory.DetectPartitionTable(disk);
                if (table != null)
                {
                    tables.Add(table);
                }
            }

            return tables;
        }

        /// <summary>
        /// Gets all of the partition tables found on a disk.
        /// </summary>
        /// <param name="contentStream">The content of the disk to inspect.</param>
        /// <returns>It is rare for a disk to have multiple partition tables, but theoretically
        /// possible.</returns>
        public static IList<PartitionTable> GetPartitionTables(Stream contentStream)
        {
            return GetPartitionTables(new Disk(contentStream, Ownership.None));
        }

        /// <summary>
        /// Creates a new partition that encompasses the entire disk.
        /// </summary>
        /// <param name="type">The partition type.</param>
        /// <param name="active">Whether the partition is active (bootable).</param>
        /// <returns>The index of the partition.</returns>
        /// <remarks>The partition table must be empty before this method is called,
        /// otherwise IOException is thrown.</remarks>
        public abstract int Create(WellKnownPartitionType type, bool active);

        /// <summary>
        /// Creates a new partition with a target size.
        /// </summary>
        /// <param name="size">The target size (in bytes).</param>
        /// <param name="type">The partition type.</param>
        /// <param name="active">Whether the partition is active (bootable).</param>
        /// <returns>The index of the new partition.</returns>
        public abstract int Create(long size, WellKnownPartitionType type, bool active);

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
        public abstract int CreateAligned(WellKnownPartitionType type, bool active, int alignment);

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
        public abstract int CreateAligned(long size, WellKnownPartitionType type, bool active, int alignment);

        /// <summary>
        /// Deletes a partition at a given index.
        /// </summary>
        /// <param name="index">The index of the partition.</param>
        public abstract void Delete(int index);
    }
}