namespace DiscUtils.Ntfs
{
    internal class DirectoryEntry
    {
        private readonly Directory _directory;

        public DirectoryEntry(Directory directory, FileRecordReference fileReference, FileNameRecord fileDetails)
        {
            _directory = directory;
            Reference = fileReference;
            Details = fileDetails;
        }

        public FileNameRecord Details { get; }

        public bool IsDirectory => (Details.Flags & FileAttributeFlags.Directory) != 0;

        public FileRecordReference Reference { get; }

        public string SearchName
        {
            get
            {
                string fileName = Details.FileName;
                if (fileName.IndexOf('.') == -1)
                {
                    return fileName + ".";
                }
                return fileName;
            }
        }

        internal void UpdateFrom(File file)
        {
            file.FreshenFileName(Details, true);
            _directory.UpdateEntry(this);
        }
    }
}