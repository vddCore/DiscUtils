using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class ObjectIds
    {
        private readonly File _file;
        private readonly IndexView<IndexKey, ObjectIdRecord> _index;

        public ObjectIds(File file)
        {
            _file = file;
            _index = new IndexView<IndexKey, ObjectIdRecord>(file.GetIndex("$O"));
        }

        internal IEnumerable<KeyValuePair<Guid, ObjectIdRecord>> All
        {
            get
            {
                foreach (KeyValuePair<IndexKey, ObjectIdRecord> record in _index.Entries)
                {
                    yield return new KeyValuePair<Guid, ObjectIdRecord>(record.Key.Id, record.Value);
                }
            }
        }

        internal void Add(Guid objId, FileRecordReference mftRef, Guid birthId, Guid birthVolumeId, Guid birthDomainId)
        {
            IndexKey newKey = new IndexKey();
            newKey.Id = objId;

            ObjectIdRecord newData = new ObjectIdRecord();
            newData.MftReference = mftRef;
            newData.BirthObjectId = birthId;
            newData.BirthVolumeId = birthVolumeId;
            newData.BirthDomainId = birthDomainId;

            _index[newKey] = newData;
            _file.UpdateRecordInMft();
        }

        internal void Remove(Guid objId)
        {
            IndexKey key = new IndexKey();
            key.Id = objId;

            _index.Remove(key);
            _file.UpdateRecordInMft();
        }

        internal bool TryGetValue(Guid objId, out ObjectIdRecord value)
        {
            IndexKey key = new IndexKey();
            key.Id = objId;

            return _index.TryGetValue(key, out value);
        }

        internal void Dump(TextWriter writer, string indent)
        {
            writer.WriteLine(indent + "OBJECT ID INDEX");

            foreach (KeyValuePair<IndexKey, ObjectIdRecord> entry in _index.Entries)
            {
                writer.WriteLine(indent + "  OBJECT ID INDEX ENTRY");
                writer.WriteLine(indent + "             Id: " + entry.Key.Id);
                writer.WriteLine(indent + "  MFT Reference: " + entry.Value.MftReference);
                writer.WriteLine(indent + "   Birth Volume: " + entry.Value.BirthVolumeId);
                writer.WriteLine(indent + "       Birth Id: " + entry.Value.BirthObjectId);
                writer.WriteLine(indent + "   Birth Domain: " + entry.Value.BirthDomainId);
            }
        }

        internal sealed class IndexKey : IByteArraySerializable
        {
            public Guid Id;

            public int Size => 16;

            public int ReadFrom(byte[] buffer, int offset)
            {
                Id = EndianUtilities.ToGuidLittleEndian(buffer, offset + 0);
                return 16;
            }

            public void WriteTo(byte[] buffer, int offset)
            {
                EndianUtilities.WriteBytesLittleEndian(Id, buffer, offset + 0);
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "[Key-Id:{0}]", Id);
            }
        }
    }
}