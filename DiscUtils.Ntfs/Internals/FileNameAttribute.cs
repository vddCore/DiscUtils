using System;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// Representation of an NTFS File Name attribute.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each Master File Table entry (MFT Entry) has one of these attributes for each
    /// hard link.  Files with a long name and a short name will have at least two of
    /// these attributes.</para>
    /// <para>
    /// The details in this attribute may be inconsistent with similar information in
    /// the StandardInformationAttribute for a file.  The StandardInformation is
    /// definitive, this attribute holds a 'cache' of the information.
    /// </para>
    /// </remarks>
    public sealed class FileNameAttribute : GenericAttribute
    {
        private readonly FileNameRecord _fnr;

        internal FileNameAttribute(INtfsContext context, AttributeRecord record)
            : base(context, record)
        {
            byte[] content = StreamUtilities.ReadAll(Content);
            _fnr = new FileNameRecord();
            _fnr.ReadFrom(content, 0);
        }

        /// <summary>
        /// Gets the amount of disk space allocated for the file.
        /// </summary>
        public long AllocatedSize => (long)_fnr.AllocatedSize;

        /// <summary>
        /// Gets the creation time of the file.
        /// </summary>
        public DateTime CreationTime => _fnr.CreationTime;

        /// <summary>
        /// Gets the extended attributes size, or a reparse tag, depending on the nature of the file.
        /// </summary>
        public long ExtendedAttributesSizeOrReparsePointTag => _fnr.EASizeOrReparsePointTag;

        /// <summary>
        /// Gets the attributes of the file, as stored by NTFS.
        /// </summary>
        public NtfsFileAttributes FileAttributes => (NtfsFileAttributes)_fnr.Flags;

        /// <summary>
        /// Gets the name of the file within the parent directory.
        /// </summary>
        public string FileName => _fnr.FileName;

        /// <summary>
        /// Gets the namespace of the FileName property.
        /// </summary>
        public NtfsNamespace FileNameNamespace => (NtfsNamespace)_fnr.FileNameNamespace;

        /// <summary>
        /// Gets the last access time of the file.
        /// </summary>
        public DateTime LastAccessTime => _fnr.LastAccessTime;

        /// <summary>
        /// Gets the last time the Master File Table entry for the file was changed.
        /// </summary>
        public DateTime MasterFileTableChangedTime => _fnr.MftChangedTime;

        /// <summary>
        /// Gets the modification time of the file.
        /// </summary>
        public DateTime ModificationTime => _fnr.ModificationTime;

        /// <summary>
        /// Gets the reference to the parent directory.
        /// </summary>
        /// <remarks>
        /// This attribute stores the name of a file within a directory, this field
        /// provides the link back to the directory.
        /// </remarks>
        public MasterFileTableReference ParentDirectory => new MasterFileTableReference(_fnr.ParentDirectory);

        /// <summary>
        /// Gets the amount of data stored in the file.
        /// </summary>
        public long RealSize => (long)_fnr.RealSize;
    }
}