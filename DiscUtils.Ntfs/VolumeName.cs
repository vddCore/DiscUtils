using System.IO;
using System.Text;
using DiscUtils.Core;
using DiscUtils.Streams;

namespace DiscUtils.Ntfs
{
    internal sealed class VolumeName : IByteArraySerializable, IDiagnosticTraceable
    {
        public VolumeName() {}

        public VolumeName(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public int Size => Encoding.Unicode.GetByteCount(Name);

        public int ReadFrom(byte[] buffer, int offset)
        {
            Name = Encoding.Unicode.GetString(buffer, offset, buffer.Length - offset);
            return buffer.Length - offset;
        }

        public void WriteTo(byte[] buffer, int offset)
        {
            Encoding.Unicode.GetBytes(Name, 0, Name.Length, buffer, offset);
        }

        public void Dump(TextWriter writer, string indent)
        {
            writer.WriteLine(indent + "  Volume Name: " + Name);
        }
    }
}