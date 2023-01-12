using DiscUtils.Streams.Buffer;

namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// Base class for all attributes within Master File Table entries.
    /// </summary>
    /// <remarks>
    /// More specialized base classes are provided for known attribute types.
    /// </remarks>
    public abstract class GenericAttribute
    {
        private readonly INtfsContext _context;
        private readonly AttributeRecord _record;

        internal GenericAttribute(INtfsContext context, AttributeRecord record)
        {
            _context = context;
            _record = record;
        }

        /// <summary>
        /// Gets the type of the attribute.
        /// </summary>
        public AttributeType AttributeType => _record.AttributeType;

        /// <summary>
        /// Gets a buffer that can access the content of the attribute.
        /// </summary>
        public IBuffer Content
        {
            get
            {
                IBuffer rawBuffer = _record.GetReadOnlyDataBuffer(_context);
                return new SubBuffer(rawBuffer, 0, _record.DataLength);
            }
        }

        /// <summary>
        /// Gets the amount of valid data in the attribute's content.
        /// </summary>
        public long ContentLength => _record.DataLength;

        /// <summary>
        /// Gets the flags indicating how the content of the attribute is stored.
        /// </summary>
        public AttributeFlags Flags => (AttributeFlags)_record.Flags;

        /// <summary>
        /// Gets the unique id of the attribute.
        /// </summary>
        public int Identifier => _record.AttributeId;

        /// <summary>
        /// Gets a value indicating whether the attribute content is stored in the MFT record itself.
        /// </summary>
        public bool IsResident => !_record.IsNonResident;

        /// <summary>
        /// Gets the name of the attribute (if any).
        /// </summary>
        public string Name => _record.Name;

        internal static GenericAttribute FromAttributeRecord(INtfsContext context, AttributeRecord record)
        {
            switch (record.AttributeType)
            {
                case AttributeType.AttributeList:
                    return new AttributeListAttribute(context, record);
                case AttributeType.FileName:
                    return new FileNameAttribute(context, record);
                case AttributeType.StandardInformation:
                    return new StandardInformationAttribute(context, record);
                default:
                    return new UnknownAttribute(context, record);
            }
        }
    }
}