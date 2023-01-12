using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class NtfsFileStream : SparseStream
    {
        private SparseStream _baseStream;
        private readonly DirectoryEntry _entry;

        private readonly File _file;

        private bool _isDirty;

        public NtfsFileStream(NtfsFileSystem fileSystem, DirectoryEntry entry, AttributeType attrType, string attrName,
                              FileAccess access)
        {
            _entry = entry;

            _file = fileSystem.GetFile(entry.Reference);
            _baseStream = _file.OpenStream(attrType, attrName, access);
        }

        public override bool CanRead
        {
            get
            {
                AssertOpen();
                return _baseStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                AssertOpen();
                return _baseStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                AssertOpen();
                return _baseStream.CanWrite;
            }
        }

        public override IEnumerable<StreamExtent> Extents
        {
            get
            {
                AssertOpen();
                return _baseStream.Extents;
            }
        }

        public override long Length
        {
            get
            {
                AssertOpen();
                return _baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                AssertOpen();
                return _baseStream.Position;
            }

            set
            {
                AssertOpen();
                using (new NtfsTransaction())
                {
                    _baseStream.Position = value;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_baseStream == null)
            {
                base.Dispose(disposing);
                return;
            }

            using (new NtfsTransaction())
            {
                base.Dispose(disposing);
                _baseStream.Dispose();

                UpdateMetadata();

                _baseStream = null;
            }
        }

        public override void Flush()
        {
            AssertOpen();
            using (new NtfsTransaction())
            {
                _baseStream.Flush();

                UpdateMetadata();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            AssertOpen();
            StreamUtilities.AssertBufferParameters(buffer, offset, count);

            using (new NtfsTransaction())
            {
                return _baseStream.Read(buffer, offset, count);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            AssertOpen();
            using (new NtfsTransaction())
            {
                return _baseStream.Seek(offset, origin);
            }
        }

        public override void SetLength(long value)
        {
            AssertOpen();
            using (new NtfsTransaction())
            {
                if (value != Length)
                {
                    _isDirty = true;
                    _baseStream.SetLength(value);
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            AssertOpen();
            StreamUtilities.AssertBufferParameters(buffer, offset, count);

            using (new NtfsTransaction())
            {
                _isDirty = true;
                _baseStream.Write(buffer, offset, count);
            }
        }

        public override void Clear(int count)
        {
            AssertOpen();
            using (new NtfsTransaction())
            {
                _isDirty = true;
                _baseStream.Clear(count);
            }
        }

        private void UpdateMetadata()
        {
            if (!_file.Context.ReadOnly)
            {
                // Update the standard information attribute - so it reflects the actual file state
                if (_isDirty)
                {
                    _file.Modified();
                }
                else
                {
                    _file.Accessed();
                }

                // Update the directory entry used to open the file, so it's accurate
                _entry.UpdateFrom(_file);

                // Write attribute changes back to the Master File Table
                _file.UpdateRecordInMft();
                _isDirty = false;
            }
        }

        private void AssertOpen()
        {
            if (_baseStream == null)
            {
                throw new ObjectDisposedException(_entry.Details.FileName, "Attempt to use closed stream");
            }
        }
    }
}