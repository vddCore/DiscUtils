using System;
using System.IO;
using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal sealed class ObjectId : IByteArraySerializable, IDiagnosticTraceable
    {
        public Guid Id;

        public int Size => 16;

        public int ReadFrom(byte[] buffer, int offset)
        {
            Id = EndianUtilities.ToGuidLittleEndian(buffer, offset);
            return 16;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            EndianUtilities.WriteBytesLittleEndian(Id, buffer, offset);
        }

        public void Dump(TextWriter writer, string indent)
        {
            writer.WriteLine(indent + "  Object ID: " + Id);
        }
    }
}