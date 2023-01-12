namespace DiscUtils.Ntfs.Internals
{
    public sealed class MasterFileTableRecord
    {
        private readonly FileRecord _fileRecord;

        internal MasterFileTableRecord(FileRecord fileRecord)
        {
            _fileRecord = fileRecord;
        }

        public MasterFileTableReference BaseRecordReference => new MasterFileTableReference(_fileRecord.BaseFile);

        public MasterFileTableRecordFlags Flags => (MasterFileTableRecordFlags)_fileRecord.Flags;

        public int HardLinkCount => _fileRecord.HardLinkCount;

        /// <summary>
        /// Changes each time the file is modified by Windows, relates to the NTFS journal.
        /// </summary>
        public long JournalSequenceNumber => (long)_fileRecord.LogFileSequenceNumber;

        public int NextAttributeId => _fileRecord.NextAttributeId;

        public int SequenceNumber => _fileRecord.SequenceNumber;
    }
}