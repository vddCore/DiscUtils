using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Vhd
{
    /// <summary>
    /// Provides read access to detailed information about a VHD file.
    /// </summary>
    public class DiskImageFileInfo
    {
        private readonly Footer _footer;
        private readonly DynamicHeader _header;
        private readonly Stream _vhdStream;

        internal DiskImageFileInfo(Footer footer, DynamicHeader header, Stream vhdStream)
        {
            _footer = footer;
            _header = header;
            _vhdStream = vhdStream;
        }

        /// <summary>
        /// Gets the cookie indicating this is a VHD file (should be "conectix").
        /// </summary>
        public string Cookie => _footer.Cookie;

        /// <summary>
        /// Gets the time the file was created (note: this is not the modification time).
        /// </summary>
        public DateTime CreationTimestamp => _footer.Timestamp;

        /// <summary>
        /// Gets the application used to create the file.
        /// </summary>
        public string CreatorApp => _footer.CreatorApp;

        /// <summary>
        /// Gets the host operating system of the application used to create the file.
        /// </summary>
        public string CreatorHostOS => _footer.CreatorHostOS;

        /// <summary>
        /// Gets the version of the application used to create the file, packed as an integer.
        /// </summary>
        public int CreatorVersion => (int)_footer.CreatorVersion;

        /// <summary>
        /// Gets the current size of the disk (in bytes).
        /// </summary>
        public long CurrentSize => _footer.CurrentSize;

        /// <summary>
        /// Gets the type of the disk.
        /// </summary>
        public FileType DiskType => _footer.DiskType;

        /// <summary>
        /// Gets the number of sparse blocks the file is divided into.
        /// </summary>
        public long DynamicBlockCount => _header.MaxTableEntries;

        /// <summary>
        /// Gets the size of a sparse allocation block, in bytes.
        /// </summary>
        public long DynamicBlockSize => _header.BlockSize;

        /// <summary>
        /// Gets the checksum value of the dynamic header structure.
        /// </summary>
        public int DynamicChecksum => (int)_header.Checksum;

        /// <summary>
        /// Gets the cookie indicating a dynamic disk header (should be "cxsparse").
        /// </summary>
        public string DynamicCookie => _header.Cookie;

        /// <summary>
        /// Gets the version of the dynamic header structure, packed as an integer.
        /// </summary>
        public int DynamicHeaderVersion => (int)_header.HeaderVersion;

        /// <summary>
        /// Gets the stored paths to the parent file (for differencing disks).
        /// </summary>
        public IEnumerable<string> DynamicParentLocators
        {
            get
            {
                List<string> vals = new List<string>(8);
                foreach (ParentLocator pl in _header.ParentLocators)
                {
                    if (pl.PlatformCode == ParentLocator.PlatformCodeWindowsAbsoluteUnicode
                        || pl.PlatformCode == ParentLocator.PlatformCodeWindowsRelativeUnicode)
                    {
                        _vhdStream.Position = pl.PlatformDataOffset;
                        byte[] buffer = StreamUtilities.ReadExact(_vhdStream, pl.PlatformDataLength);
                        vals.Add(Encoding.Unicode.GetString(buffer));
                    }
                }

                return vals;
            }
        }

        /// <summary>
        /// Gets the modification timestamp of the parent file (for differencing disks).
        /// </summary>
        public DateTime DynamicParentTimestamp => _header.ParentTimestamp;

        /// <summary>
        /// Gets the unicode name of the parent file (for differencing disks).
        /// </summary>
        public string DynamicParentUnicodeName => _header.ParentUnicodeName;

        /// <summary>
        /// Gets the unique id of the parent file (for differencing disks).
        /// </summary>
        public Guid DynamicParentUniqueId => _header.ParentUniqueId;

        /// <summary>
        /// Gets the Features bit field.
        /// </summary>
        public int Features => (int)_footer.Features;

        /// <summary>
        /// Gets the file format version packed as an integer.
        /// </summary>
        public int FileFormatVersion => (int)_footer.FileFormatVersion;

        /// <summary>
        /// Gets the checksum of the file's 'footer'.
        /// </summary>
        public int FooterChecksum => (int)_footer.Checksum;

        /// <summary>
        /// Gets the geometry of the disk.
        /// </summary>
        public Geometry Geometry => _footer.Geometry;

        /// <summary>
        /// Gets the original size of the disk (in bytes).
        /// </summary>
        public long OriginalSize => _footer.OriginalSize;

        /// <summary>
        /// Gets a flag indicating if the disk has associated saved VM memory state.
        /// </summary>
        public byte SavedState => _footer.SavedState;

        /// <summary>
        /// Gets the unique identity of this disk.
        /// </summary>
        public Guid UniqueId => _footer.UniqueId;
    }
}