using System.IO;

namespace DiscUtils.Core.Compression
{
    /// <summary>
    /// Converts a byte stream into a bit stream.
    /// </summary>
    internal class BigEndianBitStream : BitStream
    {
        private uint _buffer;
        private int _bufferAvailable;
        private readonly Stream _byteStream;

        private readonly byte[] _readBuffer = new byte[2];

        public BigEndianBitStream(Stream byteStream)
        {
            _byteStream = byteStream;
        }

        public override int MaxReadAhead => 16;

        public override uint Read(int count)
        {
            if (count > 16)
            {
                uint result = Read(16) << (count - 16);
                return result | Read(count - 16);
            }

            EnsureBufferFilled();

            _bufferAvailable -= count;

            uint mask = (uint)((1 << count) - 1);

            return (_buffer >> _bufferAvailable) & mask;
        }

        public override uint Peek(int count)
        {
            EnsureBufferFilled();

            uint mask = (uint)((1 << count) - 1);

            return (_buffer >> (_bufferAvailable - count)) & mask;
        }

        public override void Consume(int count)
        {
            EnsureBufferFilled();

            _bufferAvailable -= count;
        }

        private void EnsureBufferFilled()
        {
            if (_bufferAvailable < 16)
            {
                _readBuffer[0] = 0;
                _readBuffer[1] = 0;
                _byteStream.Read(_readBuffer, 0, 2);

                _buffer = _buffer << 16 | (uint)(_readBuffer[0] << 8) | _readBuffer[1];
                _bufferAvailable += 16;
            }
        }
    }
}