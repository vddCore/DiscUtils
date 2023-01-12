using System;

namespace DiscUtils.Core.Compression
{
    internal sealed class InverseBurrowsWheeler : DataBlockTransform
    {
        private readonly int[] _nextPos;
        private readonly int[] _pointers;

        public InverseBurrowsWheeler(int bufferSize)
        {
            _pointers = new int[bufferSize];
            _nextPos = new int[256];
        }

        protected override bool BuffersMustNotOverlap => true;

        public int OriginalIndex { get; set; }

        protected override int DoProcess(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            int outputCount = inputCount;

            // First find the frequency of each value
            Array.Clear(_nextPos, 0, _nextPos.Length);
            for (int i = inputOffset; i < inputOffset + inputCount; ++i)
            {
                _nextPos[input[i]]++;
            }

            // We know they're 'sorted' in the first column, so now can figure
            // out the position of the first instance of each.
            int sum = 0;
            for (int i = 0; i < 256; ++i)
            {
                int tempSum = sum;
                sum += _nextPos[i];
                _nextPos[i] = tempSum;
            }

            // For each value in the final column, put a pointer to to the
            // 'next' character in the first (sorted) column.
            for (int i = 0; i < inputCount; ++i)
            {
                _pointers[_nextPos[input[inputOffset + i]]++] = i;
            }

            // The 'next' character after the end of the original string is the
            // first character of the original string.
            int focus = _pointers[OriginalIndex];

            // We can now just walk the pointers to reconstruct the original string
            for (int i = 0; i < outputCount; ++i)
            {
                output[outputOffset + i] = input[inputOffset + focus];
                focus = _pointers[focus];
            }

            return outputCount;
        }

        protected override int MaxOutputCount(int inputCount)
        {
            return inputCount;
        }

        protected override int MinOutputCount(int inputCount)
        {
            return inputCount;
        }
    }
}