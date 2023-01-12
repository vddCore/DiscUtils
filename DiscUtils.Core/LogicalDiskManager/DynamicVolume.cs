using System;
using System.IO;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.LogicalDiskManager
{
    internal class DynamicVolume
    {
        private readonly DynamicDiskGroup _group;

        public byte BiosType => Record.BiosType;

        public Guid Identity { get; }

        public long Length => Record.Size * Sizes.Sector;

        private VolumeRecord Record => _group.GetVolume(Identity);

        public LogicalVolumeStatus Status => _group.GetVolumeStatus(Record.Id);

        internal DynamicVolume(DynamicDiskGroup group, Guid volumeId)
        {
            _group = group;
            Identity = volumeId;
        }

        public SparseStream Open()
        {
            if (Status == LogicalVolumeStatus.Failed)
            {
                throw new IOException("Attempt to open 'failed' volume");
            }
            return _group.OpenVolume(Record.Id);
        }
    }
}