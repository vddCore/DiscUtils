using System.IO;
using DiscUtils.Core;
using DiscUtils.Core.Vfs;
using FileSystemInfo = DiscUtils.Core.FileSystemInfo;

namespace DiscUtils.Ntfs
{
    [VfsFileSystemFactory]
    internal class FileSystemFactory : VfsFileSystemFactory
    {
        public override FileSystemInfo[] Detect(Stream stream, VolumeInfo volume)
        {
            if (NtfsFileSystem.Detect(stream))
            {
                return new FileSystemInfo[] { new VfsFileSystemInfo("NTFS", "Microsoft NTFS", Open) };
            }

            return new FileSystemInfo[0];
        }

        private DiscFileSystem Open(Stream stream, VolumeInfo volumeInfo, FileSystemParameters parameters)
        {
            return new NtfsFileSystem(stream);
        }
    }
}