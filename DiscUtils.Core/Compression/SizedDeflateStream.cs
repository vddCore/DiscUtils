using System;
using System.IO;
using System.IO.Compression;

namespace DiscUtils.Core.Compression
{
    internal class SizedDeflateStream : DeflateStream
    {
        private readonly int _length;
        private int _position;

        public SizedDeflateStream(Stream stream, CompressionMode mode, bool leaveOpen, int length)
            : base(stream, mode, leaveOpen)
        {
            _length = length;
        }

        public override long Length => _length;

        public override long Position
        {
            get => _position;
            set
            {
                if (value != Position)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public override int Read(byte[] array, int offset, int count)
        {
            int read = base.Read(array, offset, count);
            _position += read;
            return read;
        }
    }
}