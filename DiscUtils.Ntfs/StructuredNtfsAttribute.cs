using System.IO;
using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal class StructuredNtfsAttribute<T> : NtfsAttribute
        where T : IByteArraySerializable, IDiagnosticTraceable, new()
    {
        private bool _hasContent;
        private bool _initialized;
        private T _structure;

        public StructuredNtfsAttribute(File file, FileRecordReference containingFile, AttributeRecord record)
            : base(file, containingFile, record)
        {
            _structure = new T();
        }

        public T Content
        {
            get
            {
                Initialize();
                return _structure;
            }

            set
            {
                _structure = value;
                _hasContent = true;
            }
        }

        public bool HasContent
        {
            get
            {
                Initialize();
                return _hasContent;
            }
        }

        public void Save()
        {
            byte[] buffer = new byte[_structure.Size];
            _structure.WriteTo(buffer, 0);
            using (Stream s = Open(FileAccess.Write))
            {
                s.Write(buffer, 0, buffer.Length);
                s.SetLength(buffer.Length);
            }
        }

        public override string ToString()
        {
            Initialize();
            return _structure.ToString();
        }

        public override void Dump(TextWriter writer, string indent)
        {
            Initialize();
            writer.WriteLine(indent + AttributeTypeName + " ATTRIBUTE (" + (Name == null ? "No Name" : Name) + ")");
            _structure.Dump(writer, indent + "  ");

            _primaryRecord.Dump(writer, indent + "  ");
        }

        private void Initialize()
        {
            if (!_initialized)
            {
                using (Stream s = Open(FileAccess.Read))
                {
                    byte[] buffer = StreamUtilities.ReadExact(s, (int)Length);
                    _structure.ReadFrom(buffer, 0);
                    _hasContent = s.Length != 0;
                }

                _initialized = true;
            }
        }
    }
}