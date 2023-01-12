using System.Collections.Generic;
using System.IO;
using DiscUtils.Streams.Util;

namespace DiscUtils.Streams
{
    /// <summary>
    /// Base class for streams that wrap another stream.
    /// </summary>
    /// <remarks>
    /// Provides the default implementation of methods &amp; properties, so
    /// wrapping streams need only override the methods they need to intercept.
    /// </remarks>
    public class WrappingStream : SparseStream
    {
        private readonly Ownership _ownership;
        private SparseStream _wrapped;

        public WrappingStream(SparseStream toWrap, Ownership ownership)
        {
            _wrapped = toWrap;
            _ownership = ownership;
        }

        public override bool CanRead => _wrapped.CanRead;

        public override bool CanSeek => _wrapped.CanSeek;

        public override bool CanWrite => _wrapped.CanWrite;

        public override IEnumerable<StreamExtent> Extents => _wrapped.Extents;

        public override long Length => _wrapped.Length;

        public override long Position
        {
            get => _wrapped.Position;
            set => _wrapped.Position = value;
        }

        public override void Flush()
        {
            _wrapped.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _wrapped.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _wrapped.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _wrapped.SetLength(value);
        }

        public override void Clear(int count)
        {
            _wrapped.Clear(count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _wrapped.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_wrapped != null && _ownership == Ownership.Dispose)
                    {
                        _wrapped.Dispose();
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