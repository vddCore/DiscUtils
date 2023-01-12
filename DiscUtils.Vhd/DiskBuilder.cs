using System;
using System.Collections.Generic;
using System.IO;
using DiscUtils.Core;
using DiscUtils.Streams;
using DiscUtils.Streams.Builder;
using DiscUtils.Streams.Util;

namespace DiscUtils.Vhd
{
    /// <summary>
    /// Creates new VHD disks by wrapping existing streams.
    /// </summary>
    /// <remarks>Using this method for creating virtual disks avoids consuming
    /// large amounts of memory, or going via the local file system when the aim
    /// is simply to present a VHD version of an existing disk.</remarks>
    public sealed class DiskBuilder : DiskImageBuilder
    {
        /// <summary>
        /// Gets or sets the type of VHD file to build.
        /// </summary>
        public FileType DiskType { get; set; } = FileType.Dynamic;

        /// <summary>
        /// Initiates the build process.
        /// </summary>
        /// <param name="baseName">The base name for the VHD, for example 'foo' to create 'foo.vhd'.</param>
        /// <returns>A set of one or more logical files that constitute the VHD.  The first file is
        /// the 'primary' file that is normally attached to VMs.</returns>
        public override DiskImageFileSpecification[] Build(string baseName)
        {
            if (string.IsNullOrEmpty(baseName))
            {
                throw new ArgumentException("Invalid base file name", nameof(baseName));
            }

            if (Content == null)
            {
                throw new InvalidOperationException("No content stream specified");
            }

            List<DiskImageFileSpecification> fileSpecs = new List<DiskImageFileSpecification>();

            Geometry geometry = Geometry ?? Geometry.FromCapacity(Content.Length);

            Footer footer = new Footer(geometry, Content.Length, DiskType);

            if (DiskType == FileType.Fixed)
            {
                footer.UpdateChecksum();

                byte[] footerSector = new byte[Sizes.Sector];
                footer.ToBytes(footerSector, 0);

                SparseStream footerStream = SparseStream.FromStream(new MemoryStream(footerSector, false),
                    Ownership.None);
                Stream imageStream = new ConcatStream(Ownership.None, Content, footerStream);
                fileSpecs.Add(new DiskImageFileSpecification(baseName + ".vhd",
                    new PassthroughStreamBuilder(imageStream)));
            }
            else if (DiskType == FileType.Dynamic)
            {
                fileSpecs.Add(new DiskImageFileSpecification(baseName + ".vhd",
                    new DynamicDiskBuilder(Content, footer, (uint)Sizes.OneMiB * 2)));
            }
            else
            {
                throw new InvalidOperationException("Only Fixed and Dynamic disk types supported");
            }

            return fileSpecs.ToArray();
        }
    }
}