using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class IndexRoot : IByteArraySerializable, IDiagnosticTraceable
    {
        public const int HeaderOffset = 0x10;

        public uint AttributeType { get; set; }

        public AttributeCollationRule CollationRule { get; set; }

        public uint IndexAllocationSize { get; set; }

        public byte RawClustersPerIndexRecord { get; set; }

        public int Size => 16;

        public int ReadFrom(byte[] buffer, int offset)
        {
            AttributeType = EndianUtilities.ToUInt32LittleEndian(buffer, 0x00);
            CollationRule = (AttributeCollationRule)EndianUtilities.ToUInt32LittleEndian(buffer, 0x04);
            IndexAllocationSize = EndianUtilities.ToUInt32LittleEndian(buffer, 0x08);
            RawClustersPerIndexRecord = buffer[0x0C];
            return 16;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            EndianUtilities.WriteBytesLittleEndian(AttributeType, buffer, 0);
            EndianUtilities.WriteBytesLittleEndian((uint)CollationRule, buffer, 0x04);
            EndianUtilities.WriteBytesLittleEndian(IndexAllocationSize, buffer, 0x08);
            EndianUtilities.WriteBytesLittleEndian(RawClustersPerIndexRecord, buffer, 0x0C);
        }

        public void Dump(TextWriter writer, string indent)
        {
            writer.WriteLine(indent + "                Attr Type: " + AttributeType);
            writer.WriteLine(indent + "           Collation Rule: " + CollationRule);
            writer.WriteLine(indent + "         Index Alloc Size: " + IndexAllocationSize);
            writer.WriteLine(indent + "  Raw Clusters Per Record: " + RawClustersPerIndexRecord);
        }

        public IComparer<byte[]> GetCollator(UpperCase upCase)
        {
            switch (CollationRule)
            {
                case AttributeCollationRule.Filename:
                    return new FileNameComparer(upCase);
                case AttributeCollationRule.SecurityHash:
                    return new SecurityHashComparer();
                case AttributeCollationRule.UnsignedLong:
                    return new UnsignedLongComparer();
                case AttributeCollationRule.MultipleUnsignedLongs:
                    return new MultipleUnsignedLongComparer();
                case AttributeCollationRule.Sid:
                    return new SidComparer();
                default:
                    throw new NotImplementedException();
            }
        }

        private sealed class SecurityHashComparer : IComparer<byte[]>
        {
            public int Compare(byte[] x, byte[] y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (y == null)
                {
                    return -1;
                }
                if (x == null)
                {
                    return 1;
                }

                uint xHash = EndianUtilities.ToUInt32LittleEndian(x, 0);
                uint yHash = EndianUtilities.ToUInt32LittleEndian(y, 0);

                if (xHash < yHash)
                {
                    return -1;
                }
                if (xHash > yHash)
                {
                    return 1;
                }

                uint xId = EndianUtilities.ToUInt32LittleEndian(x, 4);
                uint yId = EndianUtilities.ToUInt32LittleEndian(y, 4);
                if (xId < yId)
                {
                    return -1;
                }
                if (xId > yId)
                {
                    return 1;
                }
                return 0;
            }
        }

        private sealed class UnsignedLongComparer : IComparer<byte[]>
        {
            public int Compare(byte[] x, byte[] y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (y == null)
                {
                    return -1;
                }
                if (x == null)
                {
                    return 1;
                }

                uint xVal = EndianUtilities.ToUInt32LittleEndian(x, 0);
                uint yVal = EndianUtilities.ToUInt32LittleEndian(y, 0);

                if (xVal < yVal)
                {
                    return -1;
                }
                if (xVal > yVal)
                {
                    return 1;
                }

                return 0;
            }
        }

        private sealed class MultipleUnsignedLongComparer : IComparer<byte[]>
        {
            public int Compare(byte[] x, byte[] y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }

                if (y == null)
                {
                    return -1;
                }

                if (x == null)
                {
                    return 1;
                }

                for (int i = 0; i < x.Length / 4; ++i)
                {
                    uint xVal = EndianUtilities.ToUInt32LittleEndian(x, i * 4);
                    uint yVal = EndianUtilities.ToUInt32LittleEndian(y, i * 4);

                    if (xVal < yVal)
                    {
                        return -1;
                    }
                    if (xVal > yVal)
                    {
                        return 1;
                    }
                }

                return 0;
            }
        }

        private sealed class FileNameComparer : IComparer<byte[]>
        {
            private readonly UpperCase _stringComparer;

            public FileNameComparer(UpperCase upCase)
            {
                _stringComparer = upCase;
            }

            public int Compare(byte[] x, byte[] y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (y == null)
                {
                    return -1;
                }
                if (x == null)
                {
                    return 1;
                }

                byte xFnLen = x[0x40];
                byte yFnLen = y[0x40];

                return _stringComparer.Compare(x, 0x42, xFnLen * 2, y, 0x42, yFnLen * 2);
            }
        }

        private sealed class SidComparer : IComparer<byte[]>
        {
            public int Compare(byte[] x, byte[] y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (y == null)
                {
                    return -1;
                }
                if (x == null)
                {
                    return 1;
                }

                int toComp = Math.Min(x.Length, y.Length);
                for (int i = 0; i < toComp; ++i)
                {
                    int val = x[i] - y[i];
                    if (val != 0)
                    {
                        return val;
                    }
                }

                if (x.Length < y.Length)
                {
                    return -1;
                }
                if (x.Length > y.Length)
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}