using System.Collections.Generic;
using System.IO;
using DiscUtils.Streams.Util;

namespace DiscUtils.Streams
{
    /// <summary>
    /// Base class for streams that wrap another stream.
    /// </summary>
    /// <typeparam name="T">The type of stream to wrap.</typeparam>
    /// <remarks>
    /// Provides the default implementation of methods &amp; properties, so
    /// wrapping streams need only override the methods they need to intercept.
    /// </remarks>
    public class WrappingMappedStream<T> : MappedStream
        where T : Stream
    {
        private readonly List<StreamExtent> _extents;
        private readonly Ownership _ownership;

        public WrappingMappedStream(T toWrap, Ownership ownership, IEnumerable<StreamExtent> extents)
        {
            WrappedStream = toWrap;
            _ownership = ownership;
            if (extents != null)
            {
                _extents = new List<StreamExtent>(extents);
            }
        }

        public override bool CanRead => WrappedStream.CanRead;

        public override bool CanSeek => WrappedStream.CanSeek;

        public override bool CanWrite => WrappedStream.CanWrite;

        public override IEnumerable<StreamExtent> Extents
        {
            get
            {
                if (_extents != null)
                {
                    return _extents;
                }
                SparseStream sparse = WrappedStream as SparseStream;
                if (sparse != null)
                {
                    return sparse.Extents;
                }
                return new[] { new StreamExtent(0, WrappedStream.Length) };
            }
        }

        public override long Length => WrappedStream.Length;

        public override long Position
        {
            get => WrappedStream.Position;
            set => WrappedStream.Position = value;
        }

        protected T WrappedStream { get; private set; }

        public override IEnumerable<StreamExtent> MapContent(long start, long length)
        {
            MappedStream mapped = WrappedStream as MappedStream;
            if (mapped != null)
            {
                return mapped.MapContent(start, length);
            }
            return new[] { new StreamExtent(start, length) };
        }

        public override void Flush()
        {
            WrappedStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return WrappedStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return WrappedStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            WrappedStream.SetLength(value);
        }

        public override void Clear(int count)
        {
            SparseStream sparse = WrappedStream as SparseStream;
            if (sparse != null)
            {
                sparse.Clear(count);
            }
            else
            {
                base.Clear(count);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            WrappedStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (WrappedStream != null && _ownership == Ownership.Dispose)
                    {
                        WrappedStream.Dispose();
                    }

                    WrappedStream = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}