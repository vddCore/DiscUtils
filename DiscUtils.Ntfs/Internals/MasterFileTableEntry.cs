using System.Collections.Generic;

namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// An entry within the Master File Table.
    /// </summary>
    public sealed class MasterFileTableEntry
    {
        private readonly INtfsContext _context;
        private readonly FileRecord _fileRecord;

        internal MasterFileTableEntry(INtfsContext context, FileRecord fileRecord)
        {
            _context = context;
            _fileRecord = fileRecord;
        }

        /// <summary>
        /// Gets the attributes contained in this entry.
        /// </summary>
        public ICollection<GenericAttribute> Attributes
        {
            get
            {
                List<GenericAttribute> result = new List<GenericAttribute>();
                foreach (AttributeRecord attr in _fileRecord.Attributes)
                {
                    result.Add(GenericAttribute.FromAttributeRecord(_context, attr));
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the identity of the base entry for files split over multiple entries.
        /// </summary>
        /// <remarks>
        /// All entries that form part of the same file have the same value for
        /// this property.
        /// </remarks>
        public MasterFileTableReference BaseRecordReference => new MasterFileTableReference(_fileRecord.BaseFile);

        /// <summary>
        /// Gets the flags indicating the nature of the entry.
        /// </summary>
        public MasterFileTableEntryFlags Flags => (MasterFileTableEntryFlags)_fileRecord.Flags;

        /// <summary>
        /// Gets the number of hard links referencing this file.
        /// </summary>
        public int HardLinkCount => _fileRecord.HardLinkCount;

        /// <summary>
        /// Gets the index of this entry in the Master File Table.
        /// </summary>
        public long Index => _fileRecord.LoadedIndex;

        /// <summary>
        /// Gets the change identifier that is updated each time the file is modified by Windows, relates to the NTFS log file.
        /// </summary>
        /// <remarks>
        /// The NTFS log file provides journalling, preventing meta-data corruption in the event of a system crash.
        /// </remarks>
        public long LogFileSequenceNumber => (long)_fileRecord.LogFileSequenceNumber;

        /// <summary>
        /// Gets the next attribute identity that will be allocated.
        /// </summary>
        public int NextAttributeId => _fileRecord.NextAttributeId;

        /// <summary>
        /// Gets the index of this entry in the Master File Table (as stored in the entry itself).
        /// </summary>
        /// <remarks>
        /// Note - older versions of Windows did not store this value, so it may be Zero.
        /// </remarks>
        public long SelfIndex => _fileRecord.MasterFileTableIndex;

        /// <summary>
        /// Gets the revision number of the entry.
        /// </summary>
        /// <remarks>
        /// Each time an entry is allocated or de-allocated, this number is incremented by one.
        /// </remarks>
        public int SequenceNumber => _fileRecord.SequenceNumber;
    }
}