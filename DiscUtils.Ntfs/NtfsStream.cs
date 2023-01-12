using System.Collections.Generic;
using System.IO;
using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs
{
    internal class NtfsStream
    {
        private readonly File _file;

        public NtfsStream(File file, NtfsAttribute attr)
        {
            _file = file;
            Attribute = attr;
        }

        public NtfsAttribute Attribute { get; }

        public AttributeType AttributeType => Attribute.Type;

        public string Name => Attribute.Name;

        /// <summary>
        /// Gets the content of a stream.
        /// </summary>
        /// <typeparam name="T">The stream's content structure.</typeparam>
        /// <returns>The content.</returns>
        public T GetContent<T>()
            where T : IByteArraySerializable, IDiagnosticTraceable, new()
        {
            byte[] buffer;
            using (Stream s = Open(FileAccess.Read))
            {
                buffer = StreamUtilities.ReadExact(s, (int)s.Length);
            }

            T value = new T();
            value.ReadFrom(buffer, 0);
            return value;
        }

        /// <summary>
        /// Sets the content of a stream.
        /// </summary>
        /// <typeparam name="T">The stream's content structure.</typeparam>
        /// <param name="value">The new value for the stream.</param>
        public void SetContent<T>(T value)
            where T : IByteArraySerializable, IDiagnosticTraceable, new()
        {
            byte[] buffer = new byte[value.Size];
            value.WriteTo(buffer, 0);
            using (Stream s = Open(FileAccess.Write))
            {
                s.Write(buffer, 0, buffer.Length);
                s.SetLength(buffer.Length);
            }
        }

        public SparseStream Open(FileAccess access)
        {
            return Attribute.Open(access);
        }

        internal Range<long, long>[] GetClusters()
        {
            return Attribute.GetClusters();
        }

        internal StreamExtent[] GetAbsoluteExtents()
        {
            List<StreamExtent> result = new List<StreamExtent>();

            long clusterSize = _file.Context.BiosParameterBlock.BytesPerCluster;
            if (Attribute.IsNonResident)
            {
                Range<long, long>[] clusters = Attribute.GetClusters();
                foreach (Range<long, long> clusterRange in clusters)
                {
                    result.Add(new StreamExtent(clusterRange.Offset * clusterSize, clusterRange.Count * clusterSize));
                }
            }
            else
            {
                result.Add(new StreamExtent(Attribute.OffsetToAbsolutePos(0), Attribute.Length));
            }

            return result.ToArray();
        }
    }
}