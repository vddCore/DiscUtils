using System.Collections.Generic;

namespace DiscUtils.Ntfs.Internals
{
    using InternalMasterFileTable = Ntfs.MasterFileTable;

    /// <summary>
    /// Provides read-only access to the Master File Table of an NTFS file system.
    /// </summary>
    public sealed class MasterFileTable
    {
        /// <summary>
        /// Index of the Master File Table itself.
        /// </summary>
        public const long MasterFileTableIndex = 0;

        /// <summary>
        /// Index of the Master File Table Mirror file.
        /// </summary>
        public const long MasterFileTableMirrorIndex = 1;

        /// <summary>
        /// Index of the Log file.
        /// </summary>
        public const long LogFileIndex = 2;

        /// <summary>
        /// Index of the Volume file.
        /// </summary>
        public const long VolumeIndex = 3;

        /// <summary>
        /// Index of the Attribute Definition file.
        /// </summary>
        public const long AttributeDefinitionIndex = 4;

        /// <summary>
        /// Index of the Root Directory.
        /// </summary>
        public const long RootDirectoryIndex = 5;

        /// <summary>
        /// Index of the Bitmap file.
        /// </summary>
        public const long BitmapIndex = 6;

        /// <summary>
        /// Index of the Boot sector(s).
        /// </summary>
        public const long BootIndex = 7;

        /// <summary>
        /// Index of the Bad Cluster file.
        /// </summary>
        public const long BadClusterIndex = 8;

        /// <summary>
        /// Index of the Security Descriptor file.
        /// </summary>
        public const long SecureIndex = 9;

        /// <summary>
        /// Index of the Uppercase mapping file.
        /// </summary>
        public const long UppercaseIndex = 10;

        /// <summary>
        /// Index of the Optional Extensions directory.
        /// </summary>
        public const long ExtendDirectoryIndex = 11;

        /// <summary>
        /// First index available for 'normal' files.
        /// </summary>
        private const uint FirstNormalFileIndex = 24;

        private readonly INtfsContext _context;
        private readonly InternalMasterFileTable _mft;

        internal MasterFileTable(INtfsContext context, InternalMasterFileTable mft)
        {
            _context = context;
            _mft = mft;
        }

        /// <summary>
        /// Gets an entry by index.
        /// </summary>
        /// <param name="index">The index of the entry.</param>
        /// <returns>The entry.</returns>
        public MasterFileTableEntry this[long index]
        {
            get
            {
                FileRecord mftRecord = _mft.GetRecord(index, true, true);
                if (mftRecord != null)
                {
                    return new MasterFileTableEntry(_context, mftRecord);
                }
                return null;
            }
        }

        /// <summary>
        /// Enumerates all entries.
        /// </summary>
        /// <param name="filter">Filter controlling which entries are returned.</param>
        /// <returns>An enumeration of entries matching the filter.</returns>
        public IEnumerable<MasterFileTableEntry> GetEntries(EntryStates filter)
        {
            foreach (FileRecord record in _mft.Records)
            {
                EntryStates state;
                if ((record.Flags & FileRecordFlags.InUse) != 0)
                {
                    state = EntryStates.InUse;
                }
                else
                {
                    state = EntryStates.NotInUse;
                }

                if ((state & filter) != 0)
                {
                    yield return new MasterFileTableEntry(_context, record);
                }
            }
        }
    }
}