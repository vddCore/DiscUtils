using System;
using DiscUtils.Core.System;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.Archives
{
    internal sealed class TarHeader
    {
        public const int Length = 512;
        public long FileLength;
        public UnixFilePermissions FileMode;

        public string FileName;
        public int GroupId;
        public DateTime ModificationTime;
        public int OwnerId;

        public void ReadFrom(byte[] buffer, int offset)
        {
            FileName = ReadNullTerminatedString(buffer, offset + 0, 100);
            FileMode = (UnixFilePermissions)OctalToLong(ReadNullTerminatedString(buffer, offset + 100, 8));
            OwnerId = (int)OctalToLong(ReadNullTerminatedString(buffer, offset + 108, 8));
            GroupId = (int)OctalToLong(ReadNullTerminatedString(buffer, offset + 116, 8));
            FileLength = OctalToLong(ReadNullTerminatedString(buffer, offset + 124, 12));
            ModificationTime = OctalToLong(ReadNullTerminatedString(buffer, offset + 136, 12)).FromUnixTimeSeconds().DateTime;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            Array.Clear(buffer, offset, Length);

            EndianUtilities.StringToBytes(FileName, buffer, offset, 99);
            EndianUtilities.StringToBytes(LongToOctal((long)FileMode, 7), buffer, offset + 100, 7);
            EndianUtilities.StringToBytes(LongToOctal(OwnerId, 7), buffer, offset + 108, 7);
            EndianUtilities.StringToBytes(LongToOctal(GroupId, 7), buffer, offset + 116, 7);
            EndianUtilities.StringToBytes(LongToOctal(FileLength, 11), buffer, offset + 124, 11);
            EndianUtilities.StringToBytes(LongToOctal(Convert.ToUInt32((new DateTimeOffset(ModificationTime)).ToUnixTimeSeconds()), 11), buffer, offset + 136, 11);

            // Checksum
            EndianUtilities.StringToBytes(new string(' ', 8), buffer, offset + 148, 8);
            long checkSum = 0;
            for (int i = 0; i < 512; ++i)
            {
                checkSum += buffer[offset + i];
            }

            EndianUtilities.StringToBytes(LongToOctal(checkSum, 7), buffer, offset + 148, 7);
            buffer[155] = 0;
        }

        private static string ReadNullTerminatedString(byte[] buffer, int offset, int length)
        {
            return EndianUtilities.BytesToString(buffer, offset, length).TrimEnd('\0');
        }

        private static long OctalToLong(string value)
        {
            long result = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                result = result * 8 + (value[i] - '0');
            }

            return result;
        }

        private static string LongToOctal(long value, int length)
        {
            string result = string.Empty;

            while (value > 0)
            {
                result = (char)('0' + value % 8) + result;
                value = value / 8;
            }

            return new string('0', length - result.Length) + result;
        }
    }
}