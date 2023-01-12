using System.IO;
using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class VolumeInformation : IByteArraySerializable, IDiagnosticTraceable
    {
        public const int VersionNt4 = 0x0102;
        public const int VersionW2k = 0x0300;
        public const int VersionXp = 0x0301;

        private byte _majorVersion;
        private byte _minorVersion;

        public VolumeInformation() {}

        public VolumeInformation(byte major, byte minor, VolumeInformationFlags flags)
        {
            _majorVersion = major;
            _minorVersion = minor;
            Flags = flags;
        }

        public VolumeInformationFlags Flags { get; private set; }

        public int Version => _majorVersion << 8 | _minorVersion;

        public int Size => 0x0C;

        public int ReadFrom(byte[] buffer, int offset)
        {
            _majorVersion = buffer[offset + 0x08];
            _minorVersion = buffer[offset + 0x09];
            Flags = (VolumeInformationFlags)EndianUtilities.ToUInt16LittleEndian(buffer, offset + 0x0A);
            return 0x0C;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            EndianUtilities.WriteBytesLittleEndian((ulong)0, buffer, offset + 0x00);
            buffer[offset + 0x08] = _majorVersion;
            buffer[offset + 0x09] = _minorVersion;
            EndianUtilities.WriteBytesLittleEndian((ushort)Flags, buffer, offset + 0x0A);
        }

        public void Dump(TextWriter writer, string indent)
        {
            writer.WriteLine(indent + "  Version: " + _majorVersion + "." + _minorVersion);
            writer.WriteLine(indent + "    Flags: " + Flags);
        }
    }
}