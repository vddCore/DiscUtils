using System.Collections.Generic;
using DiscUtils.Core.Internal;

namespace DiscUtils.Core.LogicalDiskManager
{
    [LogicalVolumeFactory]
    internal class DynamicDiskManagerFactory : LogicalVolumeFactory
    {
        public override bool HandlesPhysicalVolume(PhysicalVolumeInfo volume)
        {
            return DynamicDiskManager.HandlesPhysicalVolume(volume);
        }

        public override void MapDisks(IEnumerable<VirtualDisk> disks, Dictionary<string, LogicalVolumeInfo> result)
        {
            DynamicDiskManager mgr = new DynamicDiskManager();

            foreach (VirtualDisk disk in disks)
            {
                if (DynamicDiskManager.IsDynamicDisk(disk))
                {
                    mgr.Add(disk);
                }
            }

            foreach (LogicalVolumeInfo vol in mgr.GetLogicalVolumes())
            {
                result.Add(vol.Identity, vol);
            }
        }
    }
}