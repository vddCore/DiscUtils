using System.IO;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal abstract class FixupRecordBase
    {
        private int _sectorSize;
        private ushort[] _updateSequenceArray;

        public FixupRecordBase(string magic, int sectorSize)
        {
            Magic = magic;
            _sectorSize = sectorSize;
        }

        public FixupRecordBase(string magic, int sectorSize, int recordLength)
        {
            Initialize(magic, sectorSize, recordLength);
        }

        public string Magic { get; private set; }

        public int Size => CalcSize();

        public ushort UpdateSequenceCount { get; private set; }

        public ushort UpdateSequenceNumber { get; private set; }

        public ushort UpdateSequenceOffset { get; private set; }

        public int UpdateSequenceSize => UpdateSequenceCount * 2;

        public void FromBytes(byte[] buffer, int offset)
        {
            FromBytes(buffer, offset, false);
        }

        public void FromBytes(byte[] buffer, int offset, bool ignoreMagic)
        {
            string diskMagic = EndianUtilities.BytesToString(buffer, offset + 0x00, 4);
            if (Magic == null)
            {
                Magic = diskMagic;
            }
            else
            {
                if (diskMagic != Magic && ignoreMagic)
                {
                    return;
                }

                if (diskMagic != Magic)
                {
                    throw new IOException("Corrupt record");
                }
            }

            UpdateSequenceOffset = EndianUtilities.ToUInt16LittleEndian(buffer, offset + 0x04);
            UpdateSequenceCount = EndianUtilities.ToUInt16LittleEndian(buffer, offset + 0x06);

            UpdateSequenceNumber = EndianUtilities.ToUInt16LittleEndian(buffer, offset + UpdateSequenceOffset);
            _updateSequenceArray = new ushort[UpdateSequenceCount - 1];
            for (int i = 0; i < _updateSequenceArray.Length; ++i)
            {
                _updateSequenceArray[i] = EndianUtilities.ToUInt16LittleEndian(buffer,
                    offset + UpdateSequenceOffset + 2 * (i + 1));
            }

            UnprotectBuffer(buffer, offset);

            Read(buffer, offset);
        }

        public void ToBytes(byte[] buffer, int offset)
        {
            UpdateSequenceOffset = Write(buffer, offset);

            ProtectBuffer(buffer, offset);

            EndianUtilities.StringToBytes(Magic, buffer, offset + 0x00, 4);
            EndianUtilities.WriteBytesLittleEndian(UpdateSequenceOffset, buffer, offset + 0x04);
            EndianUtilities.WriteBytesLittleEndian(UpdateSequenceCount, buffer, offset + 0x06);

            EndianUtilities.WriteBytesLittleEndian(UpdateSequenceNumber, buffer, offset + UpdateSequenceOffset);
            for (int i = 0; i < _updateSequenceArray.Length; ++i)
            {
                EndianUtilities.WriteBytesLittleEndian(_updateSequenceArray[i], buffer,
                    offset + UpdateSequenceOffset + 2 * (i + 1));
            }
        }

        protected void Initialize(string magic, int sectorSize, int recordLength)
        {
            Magic = magic;
            _sectorSize = sectorSize;
            UpdateSequenceCount = (ushort)(1 + MathUtilities.Ceil(recordLength, Sizes.Sector));
            UpdateSequenceNumber = 1;
            _updateSequenceArray = new ushort[UpdateSequenceCount - 1];
        }

        protected abstract void Read(byte[] buffer, int offset);

        protected abstract ushort Write(byte[] buffer, int offset);

        protected abstract int CalcSize();

        private void UnprotectBuffer(byte[] buffer, int offset)
        {
            // First do validation check - make sure the USN matches on all sectors)
            for (int i = 0; i < _updateSequenceArray.Length; ++i)
            {
                if (UpdateSequenceNumber != EndianUtilities.ToUInt16LittleEndian(buffer, offset + Sizes.Sector * (i + 1) - 2))
                {
                    throw new IOException("Corrupt file system record found");
                }
            }

            // Now replace the USNs with the actual data from the sequence array
            for (int i = 0; i < _updateSequenceArray.Length; ++i)
            {
                EndianUtilities.WriteBytesLittleEndian(_updateSequenceArray[i], buffer, offset + Sizes.Sector * (i + 1) - 2);
            }
        }

        private void ProtectBuffer(byte[] buffer, int offset)
        {
            UpdateSequenceNumber++;

            // Read in the bytes that are replaced by the USN
            for (int i = 0; i < _updateSequenceArray.Length; ++i)
            {
                _updateSequenceArray[i] = EndianUtilities.ToUInt16LittleEndian(buffer, offset + Sizes.Sector * (i + 1) - 2);
            }

            // Overwrite the bytes that are replaced with the USN
            for (int i = 0; i < _updateSequenceArray.Length; ++i)
            {
                EndianUtilities.WriteBytesLittleEndian(UpdateSequenceNumber, buffer, offset + Sizes.Sector * (i + 1) - 2);
            }
        }
    }
}