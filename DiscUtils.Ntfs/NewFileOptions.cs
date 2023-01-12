using DiscUtils.Core.WindowsSecurity.AccessControl;

namespace DiscUtils.Ntfs
{
    /// <summary>
    /// Options controlling how new NTFS files are created.
    /// </summary>
    public sealed class NewFileOptions
    {
        /// <summary>
        /// Initializes a new instance of the NewFileOptions class.
        /// </summary>
        public NewFileOptions()
        {
            Compressed = null;
            CreateShortNames = null;
            SecurityDescriptor = null;
        }

        /// <summary>
        /// Gets or sets whether the new file should be compressed.
        /// </summary>
        /// <remarks>The default (<c>null</c>) value indicates the file system default behaviour applies.</remarks>
        public bool? Compressed { get; set; }

        /// <summary>
        /// Gets or sets whether a short name should be created for the file.
        /// </summary>
        /// <remarks>The default (<c>null</c>) value indicates the file system default behaviour applies.</remarks>
        public bool? CreateShortNames { get; set; }

        /// <summary>
        /// Gets or sets the security descriptor that to set for the new file.
        /// </summary>
        /// <remarks>The default (<c>null</c>) value indicates the security descriptor is inherited.</remarks>
        public RawSecurityDescriptor SecurityDescriptor { get; set; }
    }
}