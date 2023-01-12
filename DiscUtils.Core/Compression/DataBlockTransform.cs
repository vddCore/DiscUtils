using System;
using System.Globalization;

namespace DiscUtils.Core.Compression
{
    internal abstract class DataBlockTransform
    {
        protected abstract bool BuffersMustNotOverlap { get; }

        public int Process(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset)
        {
            if (output.Length < outputOffset + (long)MinOutputCount(inputCount))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Output buffer to small, must be at least {0} bytes may need to be {1} bytes",
                        MinOutputCount(inputCount),
                        MaxOutputCount(inputCount)));
            }

            if (BuffersMustNotOverlap)
            {
                int maxOut = MaxOutputCount(inputCount);

                if (input == output
                    && (inputOffset + (long)inputCount > outputOffset)
                    && (inputOffset <= outputOffset + (long)maxOut))
                {
                    byte[] tempBuffer = new byte[maxOut];

                    int outCount = DoProcess(input, inputOffset, inputCount, tempBuffer, 0);
                    Array.Copy(tempBuffer, 0, output, outputOffset, outCount);

                    return outCount;
                }
            }

            return DoProcess(input, inputOffset, inputCount, output, outputOffset);
        }

        protected abstract int DoProcess(byte[] input, int inputOffset, int inputCount, byte[] output, int outputOffset);

        protected abstract int MaxOutputCount(int inputCount);

        protected abstract int MinOutputCount(int inputCount);
    }
}