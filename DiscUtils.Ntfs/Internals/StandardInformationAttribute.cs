using System;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// Representation of an NTFS File Name attribute.
    /// </summary>
    /// <para>
    /// The details in this attribute may be inconsistent with similar information in
    /// the FileNameAttribute(s) for a file.  This attribute is definitive, the
    /// FileNameAttribute attribute holds a 'cache' of some of the information.
    /// </para>
    public sealed class StandardInformationAttribute : GenericAttribute
    {
        private readonly StandardInformation _si;

        internal StandardInformationAttribute(INtfsContext context, AttributeRecord record)
            : base(context, record)
        {
            byte[] content = StreamUtilities.ReadAll(Content);
            _si = new StandardInformation();
            _si.ReadFrom(content, 0);
        }

        /// <summary>
        /// Gets the Unknown.
        /// </summary>
        public long ClassId => _si.ClassId;

        /// <summary>
        /// Gets the creation time of the file.
        /// </summary>
        public DateTime CreationTime => _si.CreationTime;

        /// <summary>
        /// Gets the attributes of the file, as stored by NTFS.
        /// </summary>
        public NtfsFileAttributes FileAttributes => (NtfsFileAttributes)_si.FileAttributes;

        /// <summary>
        /// Gets the last update sequence number of the file (relates to the user-readable journal).
        /// </summary>
        public long JournalSequenceNumber => (long)_si.UpdateSequenceNumber;

        /// <summary>
        /// Gets the last access time of the file.
        /// </summary>
        public DateTime LastAccessTime => _si.LastAccessTime;

        /// <summary>
        /// Gets the last time the Master File Table entry for the file was changed.
        /// </summary>
        public DateTime MasterFileTableChangedTime => _si.MftChangedTime;

        /// <summary>
        /// Gets the maximum number of file versions (normally 0).
        /// </summary>
        public long MaxVersions => _si.MaxVersions;

        /// <summary>
        /// Gets the modification time of the file.
        /// </summary>
        public DateTime ModificationTime => _si.ModificationTime;

        /// <summary>
        /// Gets the owner identity, for the purposes of quota allocation.
        /// </summary>
        public long OwnerId => _si.OwnerId;

        /// <summary>
        /// Gets the amount charged to the owners quota for this file.
        /// </summary>
        public long QuotaCharged => (long)_si.QuotaCharged;

        /// <summary>
        /// Gets the identifier of the Security Descriptor for this file.
        /// </summary>
        /// <remarks>
        /// Security Descriptors are stored in the \$Secure meta-data file.
        /// </remarks>
        public long SecurityId => _si.SecurityId;

        /// <summary>
        /// Gets the version number of the file (normally 0).
        /// </summary>
        public long Version => _si.Version;
    }
}