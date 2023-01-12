using System.IO;
using DiscUtils.Core.Partitions;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.ApplePartitionMap
{
    [PartitionTableFactory]
    internal sealed class PartitionMapFactory : PartitionTableFactory
    {
        public override bool DetectIsPartitioned(Stream s)
        {
            if (s.Length < 1024)
            {
                return false;
            }

            s.Position = 0;

            byte[] initialBytes = StreamUtilities.ReadExact(s, 1024);

            BlockZero b0 = new BlockZero();
            b0.ReadFrom(initialBytes, 0);
            if (b0.Signature != 0x4552)
            {
                return false;
            }

            PartitionMapEntry initialPart = new PartitionMapEntry(s);
            initialPart.ReadFrom(initialBytes, 512);

            return initialPart.Signature == 0x504d;
        }

        public override PartitionTable DetectPartitionTable(VirtualDisk disk)
        {
            if (!DetectIsPartitioned(disk.Content))
            {
                return null;
            }

            return new PartitionMap(disk.Content);
        }
    }
}