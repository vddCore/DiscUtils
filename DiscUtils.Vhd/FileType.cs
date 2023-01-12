namespace DiscUtils.Vhd
{
    /// <summary>
    /// The known types of VHD disks.
    /// </summary>
    public enum FileType
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Fixed-size disk, with space allocated up-front.
        /// </summary>
        Fixed = 2,

        /// <summary>
        /// Dynamic disk, allocates space as needed.
        /// </summary>
        Dynamic = 3,

        /// <summary>
        /// Differencing disk, form of dynamic disk that stores changes relative to another disk.
        /// </summary>
        Differencing = 4
    }
}