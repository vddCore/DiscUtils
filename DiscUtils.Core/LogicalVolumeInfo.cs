using System;
using DiscUtils.Streams;

namespace DiscUtils.Core
{
    /// <summary>
    /// Information about a logical disk volume, which may be backed by one or more physical volumes.
    /// </summary>
    public sealed class LogicalVolumeInfo : VolumeInfo
    {
        private Guid _guid;
        private readonly SparseStreamOpenDelegate _opener;
        private readonly PhysicalVolumeInfo _physicalVol;

        internal LogicalVolumeInfo(Guid guid, PhysicalVolumeInfo physicalVolume, SparseStreamOpenDelegate opener,
                                   long length, byte biosType, LogicalVolumeStatus status)
        {
            _guid = guid;
            _physicalVol = physicalVolume;
            _opener = opener;
            Length = length;
            BiosType = biosType;
            Status = status;
        }

        /// <summary>
        /// Gets the disk geometry of the underlying storage medium (as used in BIOS calls), may be null.
        /// </summary>
        public override Geometry BiosGeometry => _physicalVol == null ? Geometry.Null : _physicalVol.BiosGeometry;

        /// <summary>
        /// Gets the one-byte BIOS type for this volume, which indicates the content.
        /// </summary>
        public override byte BiosType { get; }

        /// <summary>
        /// The stable identity for this logical volume.
        /// </summary>
        /// <remarks>The stability of the identity depends the disk structure.
        /// In some cases the identity may include a simple index, when no other information
        /// is available.  Best practice is to add disks to the Volume Manager in a stable 
        /// order, if the stability of this identity is paramount.</remarks>
        public override string Identity
        {
            get
            {
                if (_guid != Guid.Empty)
                {
                    return "VLG" + _guid.ToString("B");
                }
                return "VLP:" + _physicalVol.Identity;
            }
        }

        /// <summary>
        /// Gets the length of the volume (in bytes).
        /// </summary>
        public override long Length { get; }

        /// <summary>
        /// Gets the disk geometry of the underlying storage medium, if any (may be Geometry.Null).
        /// </summary>
        public override Geometry PhysicalGeometry => _physicalVol == null ? Geometry.Null : _physicalVol.PhysicalGeometry;

        /// <summary>
        /// Gets the offset of this volume in the underlying storage medium, if any (may be Zero).
        /// </summary>
        public override long PhysicalStartSector => _physicalVol == null ? 0 : _physicalVol.PhysicalStartSector;

        /// <summary>
        /// Gets the status of the logical volume, indicating volume health.
        /// </summary>
        public LogicalVolumeStatus Status { get; }
        
        /// <summary>
        /// Gets the underlying physical volume info
        /// </summary>
        public PhysicalVolumeInfo PhysicalVolume => _physicalVol;

        /// <summary>
        /// Opens a stream with access to the content of the logical volume.
        /// </summary>
        /// <returns>The volume's content as a stream.</returns>
        public override SparseStream Open()
        {
            return _opener();
        }
    }
}