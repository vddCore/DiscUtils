using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Streams.Util;

namespace DiscUtils.Streams
{
    public class MirrorStream : SparseStream
    {
        private readonly bool _canRead;
        private readonly bool _canSeek;
        private readonly bool _canWrite;
        private readonly long _length;
        private readonly Ownership _ownsWrapped;
        private List<SparseStream> _wrapped;

        public MirrorStream(Ownership ownsWrapped, params SparseStream[] wrapped)
        {
            _wrapped = new List<SparseStream>(wrapped);
            _ownsWrapped = ownsWrapped;

            _canRead = _wrapped[0].CanRead;
            _canWrite = _wrapped[0].CanWrite;
            _canSeek = _wrapped[0].CanSeek;
            _length = _wrapped[0].Length;

            foreach (SparseStream stream in _wrapped)
            {
                if (stream.CanRead != _canRead || stream.CanWrite != _canWrite || stream.CanSeek != _canSeek)
                {
                    throw new ArgumentException("All mirrored streams must have the same read/write/seek permissions",
                        nameof(wrapped));
                }

                if (stream.Length != _length)
                {
                    throw new ArgumentException("All mirrored streams must have the same length", nameof(wrapped));
                }
            }
        }

        public override bool CanRead => _canRead;

        public override bool CanSeek => _canSeek;

        public override bool CanWrite => _canWrite;

        public override IEnumerable<StreamExtent> Extents => _wrapped[0].Extents;

        public override long Length => _length;

        public override long Position
        {
            get => _wrapped[0].Position;

            set => _wrapped[0].Position = value;
        }

        public override void Flush()
        {
            _wrapped[0].Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _wrapped[0].Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _wrapped[0].Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            if (value != _length)
            {
                throw new InvalidOperationException("Changing the stream length is not permitted for mirrored streams");
            }
        }

        public override void Clear(int count)
        {
            long pos = _wrapped[0].Position;

            if (pos + count > _length)
            {
                throw new IOException("Attempt to clear beyond end of mirrored stream");
            }

            foreach (SparseStream stream in _wrapped)
            {
                stream.Position = pos;
                stream.Clear(count);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            long pos = _wrapped[0].Position;

            if (pos + count > _length)
            {
                throw new IOException("Attempt to write beyond end of mirrored stream");
            }

            foreach (SparseStream stream in _wrapped)
            {
                stream.Position = pos;
                stream.Write(buffer, offset, count);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && _ownsWrapped == Ownership.Dispose && _wrapped != null)
                {
                    foreach (SparseStream stream in _wrapped)
                    {
                        stream.Dispose();
                    }

                    _wrapped = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}