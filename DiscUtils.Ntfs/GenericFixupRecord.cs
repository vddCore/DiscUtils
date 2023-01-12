using System;

namespace DiscUtils.Ntfs
{
    internal sealed class GenericFixupRecord : FixupRecordBase
    {
        private readonly int _bytesPerSector;

        public GenericFixupRecord(int bytesPerSector)
            : base(null, bytesPerSector)
        {
            _bytesPerSector = bytesPerSector;
        }

        public byte[] Content { get; private set; }

        protected override void Read(byte[] buffer, int offset)
        {
            Content = new byte[(UpdateSequenceCount - 1) * _bytesPerSector];
            Array.Copy(buffer, offset, Content, 0, Content.Length);
        }

        protected override ushort Write(byte[] buffer, int offset)
        {
            throw new NotImplementedException();
        }

        protected override int CalcSize()
        {
            throw new NotImplementedException();
        }
    }
}