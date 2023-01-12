using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Streams.Builder;
using DiscUtils.Streams.Util;

namespace DiscUtils.Core.Archives
{
    /// <summary>
    /// Builder to create UNIX Tar archive files.
    /// </summary>
    public sealed class TarFileBuilder : StreamBuilder
    {
        private readonly List<UnixBuildFileRecord> _files;

        /// <summary>
        /// Initializes a new instance of the <see cref="TarFileBuilder"/> class.
        /// </summary>
        public TarFileBuilder()
        {
            _files = new List<UnixBuildFileRecord>();
        }

        /// <summary>
        /// Add a file to the tar archive.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <param name="buffer">The file data.</param>
        public void AddFile(string name, byte[] buffer)
        {
            _files.Add(new UnixBuildFileRecord(name, buffer));
        }

        /// <summary>
        /// Add a file to the tar archive.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <param name="buffer">The file data.</param>
        /// <param name="fileMode">The access mode of the file.</param>
        /// <param name="ownerId">The uid of the owner.</param>
        /// <param name="groupId">The gid of the owner.</param>
        /// <param name="modificationTime">The modification time for the file.</param>
        public void AddFile(
            string name, byte[] buffer, UnixFilePermissions fileMode, int ownerId, int groupId,
            DateTime modificationTime)
        {
            _files.Add(new UnixBuildFileRecord(name, buffer, fileMode, ownerId, groupId, modificationTime));
        }

        /// <summary>
        /// Add a file to the tar archive.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <param name="stream">The file data.</param>
        public void AddFile(string name, Stream stream)
        {
            _files.Add(new UnixBuildFileRecord(name, stream));
        }

        /// <summary>
        /// Add a file to the tar archive.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <param name="stream">The file data.</param>
        /// <param name="fileMode">The access mode of the file.</param>
        /// <param name="ownerId">The uid of the owner.</param>
        /// <param name="groupId">The gid of the owner.</param>
        /// <param name="modificationTime">The modification time for the file.</param>
        public void AddFile(
            string name, Stream stream, UnixFilePermissions fileMode, int ownerId, int groupId,
            DateTime modificationTime)
        {
            _files.Add(new UnixBuildFileRecord(name, stream, fileMode, ownerId, groupId, modificationTime));
        }

        protected override List<BuilderExtent> FixExtents(out long totalLength)
        {
            List<BuilderExtent> result = new List<BuilderExtent>(_files.Count * 2 + 2);
            long pos = 0;

            foreach (UnixBuildFileRecord file in _files)
            {
                BuilderExtent fileContentExtent = file.Fix(pos + TarHeader.Length);

                result.Add(new TarHeaderExtent(
                    pos, file.Name, fileContentExtent.Length, file.FileMode, file.OwnerId, file.GroupId,
                    file.ModificationTime));
                pos += TarHeader.Length;

                result.Add(fileContentExtent);
                pos += MathUtilities.RoundUp(fileContentExtent.Length, 512);
            }

            // Two empty 512-byte blocks at end of tar file.
            result.Add(new BuilderBufferExtent(pos, new byte[1024]));

            totalLength = pos + 1024;
            return result;
        }
    }
}