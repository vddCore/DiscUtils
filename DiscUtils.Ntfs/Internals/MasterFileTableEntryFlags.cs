using System;

namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// Flags indicating the nature of a Master File Table entry.
    /// </summary>
    [Flags]
    public enum MasterFileTableEntryFlags
    {
        /// <summary>
        /// Default value.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// The entry is currently in use.
        /// </summary>
        InUse = 0x0001,

        /// <summary>
        /// The entry is for a directory (rather than a file).
        /// </summary>
        IsDirectory = 0x0002,

        /// <summary>
        /// The entry is for a file that forms parts of the NTFS meta-data.
        /// </summary>
        IsMetaFile = 0x0004,

        /// <summary>
        /// The entry contains index attributes.
        /// </summary>
        HasViewIndex = 0x0008
    }
}