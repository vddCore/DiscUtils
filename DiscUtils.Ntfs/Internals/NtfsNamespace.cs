namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// The known NTFS namespaces.
    /// </summary>
    /// <remarks>
    /// NTFS has multiple namespaces, indicating whether a name is the
    /// long name for a file, the short name for a file, both, or none.
    /// </remarks>
    public enum NtfsNamespace
    {
        /// <summary>
        /// Posix namespace (i.e. long name).
        /// </summary>
        Posix = 0,

        /// <summary>
        /// Windows long file name.
        /// </summary>
        Win32 = 1,

        /// <summary>
        /// DOS (8.3) file name.
        /// </summary>
        Dos = 2,

        /// <summary>
        /// File name that is both the long name and the DOS (8.3) name.
        /// </summary>
        Win32AndDos = 3
    }
}