using DiscUtils.Core.WindowsSecurity;

namespace DiscUtils.Ntfs
{
    /// <summary>
    /// Class representing NTFS formatting options.
    /// </summary>
    public sealed class NtfsFormatOptions
    {
        /// <summary>
        /// Gets or sets the NTFS bootloader code to put in the formatted file system.
        /// </summary>
        public byte[] BootCode { get; set; }

        /// <summary>
        /// Gets or sets the SID of the computer account that notionally formatted the file system.
        /// </summary>
        /// <remarks>
        /// Certain ACLs in the file system will refer to the 'local' administrator of the indicated
        /// computer account.
        /// </remarks>
        public SecurityIdentifier ComputerAccount { get; set; }
    }
}