using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Vhd
{
    internal sealed class DiskExtent : VirtualDiskExtent
    {
        private readonly DiskImageFile _file;

        public DiskExtent(DiskImageFile file)
        {
            _file = file;
        }

        public override long Capacity => _file.Capacity;

        public override bool IsSparse => _file.IsSparse;

        public override long StoredSize => _file.StoredSize;

        public override MappedStream OpenContent(SparseStream parent, Ownership ownsParent)
        {
            return _file.DoOpenContent(parent, ownsParent);
        }
    }
}