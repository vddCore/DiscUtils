using System.Collections.Generic;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal abstract class ClusterStream
    {
        public abstract long AllocatedClusterCount { get; }

        public abstract IEnumerable<Range<long, long>> StoredClusters { get; }

        public abstract bool IsClusterStored(long vcn);

        public abstract void ExpandToClusters(long numVirtualClusters, NonResidentAttributeRecord extent, bool allocate);

        public abstract void TruncateToClusters(long numVirtualClusters);

        public abstract void ReadClusters(long startVcn, int count, byte[] buffer, int offset);

        public abstract int WriteClusters(long startVcn, int count, byte[] buffer, int offset);

        public abstract int ClearClusters(long startVcn, int count);
    }
}