using System;
using System.Globalization;
using System.IO;
using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class ReparsePointRecord : IByteArraySerializable, IDiagnosticTraceable
    {
        public byte[] Content;
        public uint Tag;

        public int Size => 8 + Content.Length;

        public int ReadFrom(byte[] buffer, int offset)
        {
            Tag = EndianUtilities.ToUInt32LittleEndian(buffer, offset);
            ushort length = EndianUtilities.ToUInt16LittleEndian(buffer, offset + 4);
            Content = new byte[length];
            Array.Copy(buffer, offset + 8, Content, 0, length);
            return 8 + length;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            EndianUtilities.WriteBytesLittleEndian(Tag, buffer, offset);
            EndianUtilities.WriteBytesLittleEndian((ushort)Content.Length, buffer, offset + 4);
            EndianUtilities.WriteBytesLittleEndian((ushort)0, buffer, offset + 6);
            Array.Copy(Content, 0, buffer, offset + 8, Content.Length);
        }

        public void Dump(TextWriter writer, string linePrefix)
        {
            writer.WriteLine(linePrefix + "                Tag: " + Tag.ToString("x", CultureInfo.InvariantCulture));

            string hex = string.Empty;
            for (int i = 0; i < Math.Min(Content.Length, 32); ++i)
            {
                hex = hex + string.Format(CultureInfo.InvariantCulture, " {0:X2}", Content[i]);
            }

            writer.WriteLine(linePrefix + "               Data:" + hex + (Content.Length > 32 ? "..." : string.Empty));
        }
    }
}