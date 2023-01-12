using System.Collections.Generic;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class SparseClusterStream : ClusterStream
    {
        private readonly NtfsAttribute _attr;
        private readonly RawClusterStream _rawStream;

        public SparseClusterStream(NtfsAttribute attr, RawClusterStream rawStream)
        {
            _attr = attr;
            _rawStream = rawStream;
        }

        public override long AllocatedClusterCount => _rawStream.AllocatedClusterCount;

        public override IEnumerable<Range<long, long>> StoredClusters => _rawStream.StoredClusters;

        public override bool IsClusterStored(long vcn)
        {
            return _rawStream.IsClusterStored(vcn);
        }

        public override void ExpandToClusters(long numVirtualClusters, NonResidentAttributeRecord extent, bool allocate)
        {
            _rawStream.ExpandToClusters(CompressionStart(numVirtualClusters), extent, false);
        }

        public override void TruncateToClusters(long numVirtualClusters)
        {
            long alignedNum = CompressionStart(numVirtualClusters);
            _rawStream.TruncateToClusters(alignedNum);
            if (alignedNum != numVirtualClusters)
            {
                _rawStream.ReleaseClusters(numVirtualClusters, (int)(alignedNum - numVirtualClusters));
            }
        }

        public override void ReadClusters(long startVcn, int count, byte[] buffer, int offset)
        {
            _rawStream.ReadClusters(startVcn, count, buffer, offset);
        }

        public override int WriteClusters(long startVcn, int count, byte[] buffer, int offset)
        {
            int clustersAllocated = 0;
            clustersAllocated += _rawStream.AllocateClusters(startVcn, count);
            clustersAllocated += _rawStream.WriteClusters(startVcn, count, buffer, offset);
            return clustersAllocated;
        }

        public override int ClearClusters(long startVcn, int count)
        {
            return _rawStream.ReleaseClusters(startVcn, count);
        }

        private long CompressionStart(long vcn)
        {
            return MathUtilities.RoundUp(vcn, _attr.CompressionUnitSize);
        }
    }
}