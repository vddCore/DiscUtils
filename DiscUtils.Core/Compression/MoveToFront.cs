namespace DiscUtils.Core.Compression
{
    internal class MoveToFront
    {
        private readonly byte[] _buffer;

        public MoveToFront()
            : this(256, false) {}

        public MoveToFront(int size, bool autoInit)
        {
            _buffer = new byte[size];

            if (autoInit)
            {
                for (byte i = 0; i < size; ++i)
                {
                    _buffer[i] = i;
                }
            }
        }

        public byte Head => _buffer[0];

        public void Set(int pos, byte val)
        {
            _buffer[pos] = val;
        }

        public byte GetAndMove(int pos)
        {
            byte val = _buffer[pos];

            for (int i = pos; i > 0; --i)
            {
                _buffer[i] = _buffer[i - 1];
            }

            _buffer[0] = val;
            return val;
        }
    }
}