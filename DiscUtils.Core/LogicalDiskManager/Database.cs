using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.LogicalDiskManager
{
    internal class Database
    {
        private readonly Dictionary<ulong, DatabaseRecord> _records;
        private readonly DatabaseHeader _vmdb;

        public Database(Stream stream)
        {
            long dbStart = stream.Position;

            byte[] buffer = new byte[Sizes.Sector];
            stream.Read(buffer, 0, buffer.Length);
            _vmdb = new DatabaseHeader();
            _vmdb.ReadFrom(buffer, 0);

            stream.Position = dbStart + _vmdb.HeaderSize;

            buffer = StreamUtilities.ReadExact(stream, (int)(_vmdb.BlockSize * _vmdb.NumVBlks));

            _records = new Dictionary<ulong, DatabaseRecord>();
            for (int i = 0; i < _vmdb.NumVBlks; ++i)
            {
                DatabaseRecord rec = DatabaseRecord.ReadFrom(buffer, (int)(i * _vmdb.BlockSize));
                if (rec != null)
                {
                    _records.Add(rec.Id, rec);
                }
            }
        }

        internal IEnumerable<DiskRecord> Disks
        {
            get
            {
                foreach (DatabaseRecord record in _records.Values)
                {
                    if (record.RecordType == RecordType.Disk)
                    {
                        yield return (DiskRecord)record;
                    }
                }
            }
        }

        internal IEnumerable<VolumeRecord> Volumes
        {
            get
            {
                foreach (DatabaseRecord record in _records.Values)
                {
                    if (record.RecordType == RecordType.Volume)
                    {
                        yield return (VolumeRecord)record;
                    }
                }
            }
        }

        internal DiskGroupRecord GetDiskGroup(Guid guid)
        {
            foreach (DatabaseRecord record in _records.Values)
            {
                if (record.RecordType == RecordType.DiskGroup)
                {
                    DiskGroupRecord dgRecord = (DiskGroupRecord)record;
                    if (new Guid(dgRecord.GroupGuidString) == guid || guid == Guid.Empty)
                    {
                        return dgRecord;
                    }
                }
            }

            return null;
        }

        internal IEnumerable<ComponentRecord> GetVolumeComponents(ulong volumeId)
        {
            foreach (DatabaseRecord record in _records.Values)
            {
                if (record.RecordType == RecordType.Component)
                {
                    ComponentRecord cmpntRecord = (ComponentRecord)record;
                    if (cmpntRecord.VolumeId == volumeId)
                    {
                        yield return cmpntRecord;
                    }
                }
            }
        }

        internal IEnumerable<ExtentRecord> GetComponentExtents(ulong componentId)
        {
            foreach (DatabaseRecord record in _records.Values)
            {
                if (record.RecordType == RecordType.Extent)
                {
                    ExtentRecord extentRecord = (ExtentRecord)record;
                    if (extentRecord.ComponentId == componentId)
                    {
                        yield return extentRecord;
                    }
                }
            }
        }

        internal DiskRecord GetDisk(ulong diskId)
        {
            return (DiskRecord)_records[diskId];
        }

        internal VolumeRecord GetVolume(ulong volumeId)
        {
            return (VolumeRecord)_records[volumeId];
        }

        internal VolumeRecord GetVolume(Guid id)
        {
            return FindRecord<VolumeRecord>(r => r.VolumeGuid == id, RecordType.Volume);
        }

        internal IEnumerable<VolumeRecord> GetVolumes()
        {
            foreach (DatabaseRecord record in _records.Values)
            {
                if (record.RecordType == RecordType.Volume)
                {
                    yield return (VolumeRecord)record;
                }
            }
        }

        internal T FindRecord<T>(Predicate<T> pred, RecordType typeId)
            where T : DatabaseRecord
        {
            foreach (DatabaseRecord record in _records.Values)
            {
                if (record.RecordType == typeId)
                {
                    T t = (T)record;
                    if (pred(t))
                    {
                        return t;
                    }
                }
            }

            return null;
        }
    }
}