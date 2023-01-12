using System;
using DiscUtils.Core.System;
using DiscUtils.Streams.Builder;

namespace DiscUtils.Core.Archives
{
    internal sealed class TarHeaderExtent : BuilderBufferExtent
    {
        private readonly long _fileLength;
        private readonly string _fileName;
        private readonly int _groupId;
        private readonly UnixFilePermissions _mode;
        private readonly DateTime _modificationTime;
        private readonly int _ownerId;

        public TarHeaderExtent(long start, string fileName, long fileLength, UnixFilePermissions mode, int ownerId,
                               int groupId, DateTime modificationTime)
            : base(start, 512)
        {
            _fileName = fileName;
            _fileLength = fileLength;
            _mode = mode;
            _ownerId = ownerId;
            _groupId = groupId;
            _modificationTime = modificationTime;
        }

        public TarHeaderExtent(long start, string fileName, long fileLength)
            : this(start, fileName, fileLength, 0, 0, 0, DateTimeOffsetExtensions.UnixEpoch) {}

        protected override byte[] GetBuffer()
        {
            byte[] buffer = new byte[TarHeader.Length];

            TarHeader header = new TarHeader();
            header.FileName = _fileName;
            header.FileLength = _fileLength;
            header.FileMode = _mode;
            header.OwnerId = _ownerId;
            header.GroupId = _groupId;
            header.ModificationTime = _modificationTime;
            header.WriteTo(buffer, 0);

            return buffer;
        }
    }
}