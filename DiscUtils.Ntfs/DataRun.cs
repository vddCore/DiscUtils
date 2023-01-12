using System.Globalization;

namespace DiscUtils.Ntfs
{
    internal class DataRun
    {
        public DataRun() {}

        public DataRun(long offset, long length, bool isSparse)
        {
            RunOffset = offset;
            RunLength = length;
            IsSparse = isSparse;
        }

        public bool IsSparse { get; private set; }

        public long RunLength { get; set; }

        public long RunOffset { get; set; }

        internal int Size
        {
            get
            {
                int runLengthSize = VarLongSize(RunLength);
                int runOffsetSize = VarLongSize(RunOffset);
                return 1 + runLengthSize + runOffsetSize;
            }
        }

        public int Read(byte[] buffer, int offset)
        {
            int runOffsetSize = (buffer[offset] >> 4) & 0x0F;
            int runLengthSize = buffer[offset] & 0x0F;

            RunLength = ReadVarLong(buffer, offset + 1, runLengthSize);
            RunOffset = ReadVarLong(buffer, offset + 1 + runLengthSize, runOffsetSize);
            IsSparse = runOffsetSize == 0;

            return 1 + runLengthSize + runOffsetSize;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:+##;-##;0}[+{1}]", RunOffset, RunLength);
        }

        internal int Write(byte[] buffer, int offset)
        {
            int runLengthSize = WriteVarLong(buffer, offset + 1, RunLength);
            int runOffsetSize = IsSparse ? 0 : WriteVarLong(buffer, offset + 1 + runLengthSize, RunOffset);

            buffer[offset] = (byte)((runLengthSize & 0x0F) | ((runOffsetSize << 4) & 0xF0));

            return 1 + runLengthSize + runOffsetSize;
        }

        private static long ReadVarLong(byte[] buffer, int offset, int size)
        {
            ulong val = 0;
            bool signExtend = false;

            for (int i = 0; i < size; ++i)
            {
                byte b = buffer[offset + i];
                val = val | ((ulong)b << (i * 8));
                signExtend = (b & 0x80) != 0;
            }

            if (signExtend)
            {
                for (int i = size; i < 8; ++i)
                {
                    val = val | ((ulong)0xFF << (i * 8));
                }
            }

            return (long)val;
        }

        private static int WriteVarLong(byte[] buffer, int offset, long val)
        {
            bool isPositive = val >= 0;

            int pos = 0;
            do
            {
                buffer[offset + pos] = (byte)(val & 0xFF);
                val >>= 8;
                pos++;
            } while (val != 0 && val != -1);

            // Avoid appearing to have a negative number that is actually positive,
            // record an extra empty byte if needed.
            if (isPositive && (buffer[offset + pos - 1] & 0x80) != 0)
            {
                buffer[offset + pos] = 0;
                pos++;
            }
            else if (!isPositive && (buffer[offset + pos - 1] & 0x80) != 0x80)
            {
                buffer[offset + pos] = 0xFF;
                pos++;
            }

            return pos;
        }

        private static int VarLongSize(long val)
        {
            bool isPositive = val >= 0;
            bool lastByteHighBitSet = false;

            int len = 0;
            do
            {
                lastByteHighBitSet = (val & 0x80) != 0;
                val >>= 8;
                len++;
            } while (val != 0 && val != -1);

            if ((isPositive && lastByteHighBitSet) || (!isPositive && !lastByteHighBitSet))
            {
                len++;
            }

            return len;
        }
    }
}