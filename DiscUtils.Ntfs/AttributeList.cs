using System.Collections;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Core;
using DiscUtils.Streams;

namespace DiscUtils.Ntfs
{
    internal class AttributeList : IByteArraySerializable, IDiagnosticTraceable, ICollection<AttributeListRecord>
    {
        private readonly List<AttributeListRecord> _records;

        public AttributeList()
        {
            _records = new List<AttributeListRecord>();
        }

        public int Size
        {
            get
            {
                int total = 0;
                foreach (AttributeListRecord record in _records)
                {
                    total += record.Size;
                }

                return total;
            }
        }

        public int ReadFrom(byte[] buffer, int offset)
        {
            _records.Clear();

            int pos = 0;
            while (pos < buffer.Length)
            {
                AttributeListRecord r = new AttributeListRecord();
                pos += r.ReadFrom(buffer, offset + pos);
                _records.Add(r);
            }

            return pos;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            int pos = offset;
            foreach (AttributeListRecord record in _records)
            {
                record.WriteTo(buffer, offset + pos);
                pos += record.Size;
            }
        }

        public int Count => _records.Count;

        public bool IsReadOnly => false;

        public void Add(AttributeListRecord item)
        {
            _records.Add(item);
            _records.Sort();
        }

        public void Clear()
        {
            _records.Clear();
        }

        public bool Contains(AttributeListRecord item)
        {
            return _records.Contains(item);
        }

        public void CopyTo(AttributeListRecord[] array, int arrayIndex)
        {
            _records.CopyTo(array, arrayIndex);
        }

        public bool Remove(AttributeListRecord item)
        {
            return _records.Remove(item);
        }

        #region IEnumerable<AttributeListRecord> Members

        public IEnumerator<AttributeListRecord> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        #endregion

        public void Dump(TextWriter writer, string indent)
        {
            writer.WriteLine(indent + "ATTRIBUTE LIST RECORDS");
            foreach (AttributeListRecord r in _records)
            {
                r.Dump(writer, indent + "  ");
            }
        }
    }
}