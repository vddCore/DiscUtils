using System;
using System.IO;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal class IndexBlock : FixupRecordBase
    {
        /// <summary>
        /// Size of meta-data placed at start of a block.
        /// </summary>
        private const int FieldSize = 0x18;

        private readonly Index _index;
        private ulong _indexBlockVcn; // Virtual Cluster Number (maybe in sectors sometimes...?)
        private readonly bool _isRoot;

        private ulong _logSequenceNumber;

        private readonly long _streamPosition;

        public IndexBlock(Index index, bool isRoot, IndexEntry parentEntry, BiosParameterBlock bpb)
            : base("INDX", bpb.BytesPerSector)
        {
            _index = index;
            _isRoot = isRoot;

            Stream stream = index.AllocationStream;
            _streamPosition = index.IndexBlockVcnToPosition(parentEntry.ChildrenVirtualCluster);
            stream.Position = _streamPosition;
            byte[] buffer = StreamUtilities.ReadExact(stream, (int)index.IndexBufferSize);
            FromBytes(buffer, 0);
        }

        private IndexBlock(Index index, bool isRoot, long vcn, BiosParameterBlock bpb)
            : base("INDX", bpb.BytesPerSector, bpb.IndexBufferSize)
        {
            _index = index;
            _isRoot = isRoot;

            _indexBlockVcn = (ulong)vcn;

            _streamPosition = vcn * bpb.BytesPerSector * bpb.SectorsPerCluster;

            Node = new IndexNode(WriteToDisk, UpdateSequenceSize, _index, isRoot,
                (uint)bpb.IndexBufferSize - FieldSize);

            WriteToDisk();
        }

        public IndexNode Node { get; private set; }

        internal static IndexBlock Initialize(Index index, bool isRoot, IndexEntry parentEntry, BiosParameterBlock bpb)
        {
            return new IndexBlock(index, isRoot, parentEntry.ChildrenVirtualCluster, bpb);
        }

        internal void WriteToDisk()
        {
            byte[] buffer = new byte[_index.IndexBufferSize];
            ToBytes(buffer, 0);

            Stream stream = _index.AllocationStream;
            stream.Position = _streamPosition;
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
        }

        protected override void Read(byte[] buffer, int offset)
        {
            // Skip FixupRecord fields...
            _logSequenceNumber = EndianUtilities.ToUInt64LittleEndian(buffer, offset + 0x08);
            _indexBlockVcn = EndianUtilities.ToUInt64LittleEndian(buffer, offset + 0x10);
            Node = new IndexNode(WriteToDisk, UpdateSequenceSize, _index, _isRoot, buffer, offset + FieldSize);
        }

        protected override ushort Write(byte[] buffer, int offset)
        {
            EndianUtilities.WriteBytesLittleEndian(_logSequenceNumber, buffer, offset + 0x08);
            EndianUtilities.WriteBytesLittleEndian(_indexBlockVcn, buffer, offset + 0x10);
            return (ushort)(FieldSize + Node.WriteTo(buffer, offset + FieldSize));
        }

        protected override int CalcSize()
        {
            throw new NotImplementedException();
        }
    }
}