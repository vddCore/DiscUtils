using System;
using System.IO;
using DiscUtils.Core.System;
using DiscUtils.Streams.Builder;

namespace DiscUtils.Core.Archives
{
    internal sealed class UnixBuildFileRecord
    {
        private readonly BuilderExtentSource _source;

        public UnixBuildFileRecord(string name, byte[] buffer)
            : this(name, new BuilderBufferExtentSource(buffer), 0, 0, 0, DateTimeOffsetExtensions.UnixEpoch) {}

        public UnixBuildFileRecord(string name, Stream stream)
            : this(name, new BuilderStreamExtentSource(stream), 0, 0, 0, DateTimeOffsetExtensions.UnixEpoch) {}

        public UnixBuildFileRecord(
            string name, byte[] buffer, UnixFilePermissions fileMode, int ownerId, int groupId,
            DateTime modificationTime)
            : this(name, new BuilderBufferExtentSource(buffer), fileMode, ownerId, groupId, modificationTime) {}

        public UnixBuildFileRecord(
            string name, Stream stream, UnixFilePermissions fileMode, int ownerId, int groupId,
            DateTime modificationTime)
            : this(name, new BuilderStreamExtentSource(stream), fileMode, ownerId, groupId, modificationTime) {}

        public UnixBuildFileRecord(string name, BuilderExtentSource fileSource, UnixFilePermissions fileMode,
                                   int ownerId, int groupId, DateTime modificationTime)
        {
            Name = name;
            _source = fileSource;
            FileMode = fileMode;
            OwnerId = ownerId;
            GroupId = groupId;
            ModificationTime = modificationTime;
        }

        public UnixFilePermissions FileMode { get; }

        public int GroupId { get; }

        public DateTime ModificationTime { get; }

        public string Name { get; }

        public int OwnerId { get; }

        public BuilderExtent Fix(long pos)
        {
            return _source.Fix(pos);
        }
    }
}