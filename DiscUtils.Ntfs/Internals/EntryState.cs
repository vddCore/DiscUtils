using System;

namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// Flags indicating the state of a Master File Table entry.
    /// </summary>
    /// <remarks>
    /// Used to filter entries in the Master File Table.
    /// </remarks>
    [Flags]
    public enum EntryState
    {
        /// <summary>
        /// No entries match.
        /// </summary>
        None = 0,

        /// <summary>
        /// The entry is currently in use.
        /// </summary>
        InUse = 1,

        /// <summary>
        /// The entry is currently not in use.
        /// </summary>
        NotInUse = 2,

        /// <summary>
        /// All entries match.
        /// </summary>
        All = 3
    }
}