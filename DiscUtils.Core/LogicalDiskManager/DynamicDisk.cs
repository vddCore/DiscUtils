using System;
using System.IO;
using DiscUtils.Core.Partitions;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.LogicalDiskManager
{
    internal class DynamicDisk : IDiagnosticTraceable
    {
        private readonly VirtualDisk _disk;
        private readonly PrivateHeader _header;

        internal DynamicDisk(VirtualDisk disk)
        {
            _disk = disk;
            _header = GetPrivateHeader(_disk);

            TocBlock toc = GetTableOfContents();

            long dbStart = _header.ConfigurationStartLba * 512 + toc.Item1Start * 512;
            _disk.Content.Position = dbStart;
            Database = new Database(_disk.Content);
        }

        public SparseStream Content => _disk.Content;

        public Database Database { get; }

        public long DataOffset => _header.DataStartLba;

        public Guid GroupId => string.IsNullOrEmpty(_header.DiskGroupId) ? Guid.Empty : new Guid(_header.DiskGroupId);

        public Guid Id => new Guid(_header.DiskId);

        public void Dump(TextWriter writer, string linePrefix)
        {
            writer.WriteLine(linePrefix + "DISK (" + _header.DiskId + ")");
            writer.WriteLine(linePrefix + "      Metadata Version: " + ((_header.Version >> 16) & 0xFFFF) + "." +
                             (_header.Version & 0xFFFF));
            writer.WriteLine(linePrefix + "             Timestamp: " + _header.Timestamp);
            writer.WriteLine(linePrefix + "               Disk Id: " + _header.DiskId);
            writer.WriteLine(linePrefix + "               Host Id: " + _header.HostId);
            writer.WriteLine(linePrefix + "         Disk Group Id: " + _header.DiskGroupId);
            writer.WriteLine(linePrefix + "       Disk Group Name: " + _header.DiskGroupName);
            writer.WriteLine(linePrefix + "            Data Start: " + _header.DataStartLba + " (Sectors)");
            writer.WriteLine(linePrefix + "             Data Size: " + _header.DataSizeLba + " (Sectors)");
            writer.WriteLine(linePrefix + "   Configuration Start: " + _header.ConfigurationStartLba + " (Sectors)");
            writer.WriteLine(linePrefix + "    Configuration Size: " + _header.ConfigurationSizeLba + " (Sectors)");
            writer.WriteLine(linePrefix + "              TOC Size: " + _header.TocSizeLba + " (Sectors)");
            writer.WriteLine(linePrefix + "              Next TOC: " + _header.NextTocLba + " (Sectors)");
            writer.WriteLine(linePrefix + "     Number of Configs: " + _header.NumberOfConfigs);
            writer.WriteLine(linePrefix + "           Config Size: " + _header.ConfigurationSizeLba + " (Sectors)");
            writer.WriteLine(linePrefix + "        Number of Logs: " + _header.NumberOfLogs);
            writer.WriteLine(linePrefix + "              Log Size: " + _header.LogSizeLba + " (Sectors)");
        }

        internal static PrivateHeader GetPrivateHeader(VirtualDisk disk)
        {
            if (disk.IsPartitioned)
            {
                long headerPos = 0;
                PartitionTable pt = disk.Partitions;
                if (pt is BiosPartitionTable)
                {
                    headerPos = 0xc00;
                }
                else
                {
                    foreach (PartitionInfo part in pt.Partitions)
                    {
                        if (part.GuidType == GuidPartitionTypes.WindowsLdmMetadata)
                        {
                            headerPos = part.LastSector * Sizes.Sector;
                        }
                    }
                }

                if (headerPos != 0)
                {
                    disk.Content.Position = headerPos;
                    byte[] buffer = new byte[Sizes.Sector];
                    disk.Content.Read(buffer, 0, buffer.Length);

                    PrivateHeader hdr = new PrivateHeader();
                    hdr.ReadFrom(buffer, 0);
                    return hdr;
                }
            }

            return null;
        }

        private TocBlock GetTableOfContents()
        {
            byte[] buffer = new byte[_header.TocSizeLba * 512];
            _disk.Content.Position = _header.ConfigurationStartLba * 512 + 1 * _header.TocSizeLba * 512;

            _disk.Content.Read(buffer, 0, buffer.Length);
            TocBlock tocBlock = new TocBlock();
            tocBlock.ReadFrom(buffer, 0);

            if (tocBlock.Signature == "TOCBLOCK")
            {
                return tocBlock;
            }

            return null;
        }
    }
}