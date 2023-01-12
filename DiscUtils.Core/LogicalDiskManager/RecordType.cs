namespace DiscUtils.Core.LogicalDiskManager
{
    internal enum RecordType : byte
    {
        None = 0,
        Volume = 1,
        Component = 2,
        Extent = 3,
        Disk = 4,
        DiskGroup = 5
    }
}