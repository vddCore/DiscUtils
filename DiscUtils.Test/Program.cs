using System;
using System.IO;
using DiscUtils.Core;
using DiscUtils.Core.Partitions;
using DiscUtils.Ntfs;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;
using DiscUtils.Vhd;
using File = System.IO.File;

namespace DiscUtils.Test
{
    class Program
    {
        private static Disk Vhd { get; set; }
        private static NtfsFileSystem Ntfs { get; set; }

        static void Main(string[] args)
        {
            if (File.Exists("disk.vhd"))
            {
                File.Delete("disk.vhd");
            }
            else
            {
                var diskStream = File.Create("disk.vhd");
                Vhd = Disk.InitializeDynamic(diskStream, Ownership.None, 1024 * 1024 * 1024);
                BiosPartitionTable.Initialize(Vhd, WellKnownPartitionType.WindowsNtfs);
                var volmgr = new VolumeManager(Vhd);
                Ntfs = NtfsFileSystem.Format(volmgr.GetPhysicalVolumes()[0], "test");
            }
        }
    }
}