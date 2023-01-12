using System;
using System.IO;

namespace DiscUtils.Core.Compression
{
    internal class BZip2RleStream : Stream
    {
        private byte[] _blockBuffer;
        private int _blockOffset;
        private int _blockRemaining;
        private byte _lastByte;

        private int _numSame;
        private long _position;
        private int _runBytesOutstanding;

        public bool AtEof => _runBytesOutstanding == 0 && _blockRemaining == 0;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public void Reset(byte[] buffer, int offset, int count)
        {
            _position = 0;
            _blockBuffer = buffer;
            _blockOffset = offset;
            _blockRemaining = count;
            _numSame = -1;
            _lastByte = 0;
            _runBytesOutstanding = 0;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int numRead = 0;

            while (numRead < count && _runBytesOutstanding > 0)
            {
                int runCount = Math.Min(_runBytesOutstanding, count);
                for (int i = 0; i < runCount; ++i)
                {
                    buffer[offset + numRead] = _lastByte;
                }

                _runBytesOutstanding -= runCount;
                numRead += runCount;
            }

            while (numRead < count && _blockRemaining > 0)
            {
                byte b = _blockBuffer[_blockOffset];
                ++_blockOffset;
                --_blockRemaining;

                if (_numSame == 4)
                {
                    int runCount = Math.Min(b, count - numRead);
                    for (int i = 0; i < runCount; ++i)
                    {
                        buffer[offset + numRead] = _lastByte;
                        numRead++;
                    }

                    _runBytesOutstanding = b - runCount;
                    _numSame = 0;
                }
                else
                {
                    if (b != _lastByte || _numSame <= 0)
                    {
                        _lastByte = b;
                        _numSame = 0;
                    }

                    buffer[offset + numRead] = b;
                    numRead++;
                    _numSame++;
                }
            }

            _position += numRead;
            return numRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}