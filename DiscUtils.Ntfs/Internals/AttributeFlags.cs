using System;

namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// Flags indicating how an attribute's content is stored on disk.
    /// </summary>
    [Flags]
    public enum AttributeFlags
    {
        /// <summary>
        /// The data is stored in linear form.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// The data is compressed.
        /// </summary>
        Compressed = 0x0001,

        /// <summary>
        /// The data is encrypted.
        /// </summary>
        Encrypted = 0x4000,

        /// <summary>
        /// The data is stored in sparse form.
        /// </summary>
        Sparse = 0x8000
    }
}