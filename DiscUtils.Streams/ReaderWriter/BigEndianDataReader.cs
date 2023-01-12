using System;
using System.IO;
using DiscUtils.Streams.Util;

namespace DiscUtils.Streams.ReaderWriter
{
    public class BigEndianDataReader : DataReader
    {
        public BigEndianDataReader(Stream stream)
            : base(stream) {}

        public override ushort ReadUInt16()
        {
            ReadToBuffer(sizeof(UInt16));
            return EndianUtilities.ToUInt16BigEndian(_buffer, 0);
        }

        public override int ReadInt32()
        {
            ReadToBuffer(sizeof(Int32));
            return EndianUtilities.ToInt32BigEndian(_buffer, 0);
        }

        public override uint ReadUInt32()
        {
            ReadToBuffer(sizeof(UInt32));
            return EndianUtilities.ToUInt32BigEndian(_buffer, 0);
        }

        public override long ReadInt64()
        {
            ReadToBuffer(sizeof(Int64));
            return EndianUtilities.ToInt64BigEndian(_buffer, 0);
        }

        public override ulong ReadUInt64()
        {
            ReadToBuffer(sizeof(UInt64));
            return EndianUtilities.ToUInt64BigEndian(_buffer, 0);
        }
    }
}