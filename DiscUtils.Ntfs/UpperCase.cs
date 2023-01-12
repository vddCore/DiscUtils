using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class UpperCase : IComparer<string>
    {
        private readonly char[] _table;

        public UpperCase(File file)
        {
            using (Stream s = file.OpenStream(AttributeType.Data, null, FileAccess.Read))
            {
                _table = new char[s.Length / 2];

                byte[] buffer = StreamUtilities.ReadExact(s, (int)s.Length);

                for (int i = 0; i < _table.Length; ++i)
                {
                    _table[i] = (char)EndianUtilities.ToUInt16LittleEndian(buffer, i * 2);
                }
            }
        }

        public int Compare(string x, string y)
        {
            int compLen = Math.Min(x.Length, y.Length);
            for (int i = 0; i < compLen; ++i)
            {
                int result = _table[x[i]] - _table[y[i]];
                if (result != 0)
                {
                    return result;
                }
            }

            // Identical out to the shortest string, so length is now the
            // determining factor.
            return x.Length - y.Length;
        }

        public int Compare(byte[] x, int xOffset, int xLength, byte[] y, int yOffset, int yLength)
        {
            int compLen = Math.Min(xLength, yLength) / 2;
            for (int i = 0; i < compLen; ++i)
            {
                char xCh = (char)(x[xOffset + i * 2] | (x[xOffset + i * 2 + 1] << 8));
                char yCh = (char)(y[yOffset + i * 2] | (y[yOffset + i * 2 + 1] << 8));

                int result = _table[xCh] - _table[yCh];
                if (result != 0)
                {
                    return result;
                }
            }

            // Identical out to the shortest string, so length is now the
            // determining factor.
            return xLength - yLength;
        }

        internal static UpperCase Initialize(File file)
        {
            byte[] buffer = new byte[(char.MaxValue + 1) * 2];
            for (int i = char.MinValue; i <= char.MaxValue; ++i)
            {
                EndianUtilities.WriteBytesLittleEndian(char.ToUpperInvariant((char)i), buffer, i * 2);
            }

            using (Stream s = file.OpenStream(AttributeType.Data, null, FileAccess.ReadWrite))
            {
                s.Write(buffer, 0, buffer.Length);
            }

            return new UpperCase(file);
        }
    }
}