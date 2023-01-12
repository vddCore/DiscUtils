using System;

namespace DiscUtils.Ntfs.Internals
{
    [Flags]
    public enum MasterFileTableRecordFlags
    {
        None = 0x0000,
        InUse = 0x0001,
        IsDirectory = 0x0002,
        IsMetaFile = 0x0004,
        HasViewIndex = 0x0008
    }
}