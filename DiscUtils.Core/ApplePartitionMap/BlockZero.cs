using System;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.ApplePartitionMap
{
    internal sealed class BlockZero : IByteArraySerializable
    {
        public uint BlockCount;
        public ushort BlockSize;
        public ushort DeviceId;
        public ushort DeviceType;
        public ushort DriverCount;
        public uint DriverData;
        public ushort Signature;

        public int Size => 512;

        public int ReadFrom(byte[] buffer, int offset)
        {
            Signature = EndianUtilities.ToUInt16BigEndian(buffer, offset + 0);
            BlockSize = EndianUtilities.ToUInt16BigEndian(buffer, offset + 2);
            BlockCount = EndianUtilities.ToUInt32BigEndian(buffer, offset + 4);
            DeviceType = EndianUtilities.ToUInt16BigEndian(buffer, offset + 8);
            DeviceId = EndianUtilities.ToUInt16BigEndian(buffer, offset + 10);
            DriverData = EndianUtilities.ToUInt32BigEndian(buffer, offset + 12);
            DriverCount = EndianUtilities.ToUInt16LittleEndian(buffer, offset + 16);

            return 512;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }
    }
}