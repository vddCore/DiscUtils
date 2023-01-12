using System.IO;

namespace DiscUtils.Core.Partitions
{
    internal abstract class PartitionTableFactory
    {
        public abstract bool DetectIsPartitioned(Stream s);

        public abstract PartitionTable DetectPartitionTable(VirtualDisk disk);
    }
}