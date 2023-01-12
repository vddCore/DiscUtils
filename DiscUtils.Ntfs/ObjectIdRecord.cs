using System;
using System.Globalization;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class ObjectIdRecord : IByteArraySerializable
    {
        public Guid BirthDomainId;
        public Guid BirthObjectId;
        public Guid BirthVolumeId;
        public FileRecordReference MftReference;

        public int Size => 0x38;

        public int ReadFrom(byte[] buffer, int offset)
        {
            MftReference = new FileRecordReference();
            MftReference.ReadFrom(buffer, offset);

            BirthVolumeId = EndianUtilities.ToGuidLittleEndian(buffer, offset + 0x08);
            BirthObjectId = EndianUtilities.ToGuidLittleEndian(buffer, offset + 0x18);
            BirthDomainId = EndianUtilities.ToGuidLittleEndian(buffer, offset + 0x28);
            return 0x38;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            MftReference.WriteTo(buffer, offset);
            EndianUtilities.WriteBytesLittleEndian(BirthVolumeId, buffer, offset + 0x08);
            EndianUtilities.WriteBytesLittleEndian(BirthObjectId, buffer, offset + 0x18);
            EndianUtilities.WriteBytesLittleEndian(BirthDomainId, buffer, offset + 0x28);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[Data-MftRef:{0},BirthVolId:{1},BirthObjId:{2},BirthDomId:{3}]", MftReference, BirthVolumeId,
                BirthObjectId, BirthDomainId);
        }
    }
}