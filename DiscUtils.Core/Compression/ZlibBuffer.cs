using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;
using Buffer=DiscUtils.Streams.Buffer.Buffer;

namespace DiscUtils.Core.Compression
{
    internal class ZlibBuffer : Buffer
    {
        private Ownership _ownership;
        private readonly Stream _stream;
        private int position;

        public ZlibBuffer(Stream stream, Ownership ownership)
        {
            _stream = stream;
            _ownership = ownership;
            position = 0;
        }

        public override bool CanRead => _stream.CanRead;

        public override bool CanWrite => _stream.CanWrite;

        public override long Capacity => _stream.Length;

        public override int Read(long pos, byte[] buffer, int offset, int count)
        {
            if (pos != position)
            {
                throw new NotSupportedException();
            }

            int read = _stream.Read(buffer, offset, count);
            position += read;
            return read;
        }

        public override void Write(long pos, byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void SetCapacity(long value)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<StreamExtent> GetExtentsInRange(long start, long count)
        {
            yield return new StreamExtent(0, _stream.Length);
        }
    }
}