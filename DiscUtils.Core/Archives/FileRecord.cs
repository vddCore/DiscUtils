namespace DiscUtils.Core.Archives
{
    internal sealed class FileRecord
    {
        public long Length;
        public string Name;
        public long Start;

        public FileRecord(string name, long start, long length)
        {
            Name = name;
            Start = start;
            Length = length;
        }
    }
}