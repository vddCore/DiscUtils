using System;

namespace DiscUtils.Ntfs
{
    [Flags]
    internal enum VolumeInformationFlags : ushort
    {
        None = 0x00,
        Dirty = 0x01,
        ResizeLogFile = 0x02,
        UpgradeOnMount = 0x04,
        MountedOnNT4 = 0x08,
        DeleteUSNUnderway = 0x10,
        RepairObjectIds = 0x20,
        DisableShortNameCreation = 0x80,
        ModifiedByChkDsk = 0x8000
    }
}