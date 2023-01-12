namespace DiscUtils.Ntfs
{
    /// <summary>
    /// Controls whether short file names are created automatically.
    /// </summary>
    public enum ShortFileNameOption
    {
        /// <summary>
        /// Creates short file names, unless they've been disabled in NTFS.
        /// </summary>
        UseVolumeFlag,

        /// <summary>
        /// Does not create short names, ignoring the NTFS setting.
        /// </summary>
        Disabled,

        /// <summary>
        /// Always creates short names, ignoring the NTFS setting.
        /// </summary>
        Enabled
    }
}