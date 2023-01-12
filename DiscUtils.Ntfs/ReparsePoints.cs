using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal class ReparsePoints
    {
        private readonly File _file;
        private readonly IndexView<Key, Data> _index;

        public ReparsePoints(File file)
        {
            _file = file;
            _index = new IndexView<Key, Data>(file.GetIndex("$R"));
        }

        internal void Add(uint tag, FileRecordReference file)
        {
            Key newKey = new Key();
            newKey.Tag = tag;
            newKey.File = file;

            Data data = new Data();

            _index[newKey] = data;
            _file.UpdateRecordInMft();
        }

        internal void Remove(uint tag, FileRecordReference file)
        {
            Key key = new Key();
            key.Tag = tag;
            key.File = file;

            _index.Remove(key);
            _file.UpdateRecordInMft();
        }

        internal void Dump(TextWriter writer, string indent)
        {
            writer.WriteLine(indent + "REPARSE POINT INDEX");

            foreach (KeyValuePair<Key, Data> entry in _index.Entries)
            {
                writer.WriteLine(indent + "  REPARSE POINT INDEX ENTRY");
                writer.WriteLine(indent + "            Tag: " +
                                 entry.Key.Tag.ToString("x", CultureInfo.InvariantCulture));
                writer.WriteLine(indent + "  MFT Reference: " + entry.Key.File);
            }
        }

        internal sealed class Key : IByteArraySerializable
        {
            public FileRecordReference File;
            public uint Tag;

            public int Size => 12;

            public int ReadFrom(byte[] buffer, int offset)
            {
                Tag = EndianUtilities.ToUInt32LittleEndian(buffer, offset);
                File = new FileRecordReference(EndianUtilities.ToUInt64LittleEndian(buffer, offset + 4));
                return 12;
            }

            public void WriteTo(byte[] buffer, int offset)
            {
                EndianUtilities.WriteBytesLittleEndian(Tag, buffer, offset);
                EndianUtilities.WriteBytesLittleEndian(File.Value, buffer, offset + 4);
                ////Utilities.WriteBytesLittleEndian((uint)0, buffer, offset + 12);
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "{0:x}:", Tag) + File;
            }
        }

        internal sealed class Data : IByteArraySerializable
        {
            public int Size => 0;

            public int ReadFrom(byte[] buffer, int offset)
            {
                return 0;
            }

            public void WriteTo(byte[] buffer, int offset) {}

            public override string ToString()
            {
                return "<no data>";
            }
        }
    }
}