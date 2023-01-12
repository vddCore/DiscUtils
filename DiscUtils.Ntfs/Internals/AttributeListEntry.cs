namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// Represents an entry in an AttributeList attribute.
    /// </summary>
    /// <remarks>Each instance of this class points to the actual Master File Table
    /// entry that contains the attribute.  It is used for files split over multiple
    /// Master File Table entries.</remarks>
    public sealed class AttributeListEntry
    {
        private readonly AttributeListRecord _record;

        internal AttributeListEntry(AttributeListRecord record)
        {
            _record = record;
        }

        /// <summary>
        /// Gets the identifier of the attribute.
        /// </summary>
        public int AttributeIdentifier => _record.AttributeId;

        /// <summary>
        /// Gets the name of the attribute (if any).
        /// </summary>
        public string AttributeName => _record.Name;

        /// <summary>
        /// Gets the type of the attribute.
        /// </summary>
        public AttributeType AttributeType => _record.Type;

        /// <summary>
        /// Gets the first cluster represented in this attribute (normally 0).
        /// </summary>
        /// <remarks>
        /// <para>
        /// For very fragmented files, it can be necessary to split a single attribute
        /// over multiple Master File Table entries.  This is achieved with multiple attributes
        /// with the same name and type (one per Master File Table entry), with this field
        /// determining the logical order of the attributes.
        /// </para>
        /// <para>
        /// The number is the first 'virtual' cluster present (i.e. divide the file's content
        /// into 'cluster' sized chunks, this is the first of those clusters logically
        /// represented in the attribute).
        /// </para>
        /// </remarks>
        public long FirstFileCluster => (long)_record.StartVcn;

        /// <summary>
        /// Gets the Master File Table entry that contains the attribute.
        /// </summary>
        public MasterFileTableReference MasterFileTableEntry => new MasterFileTableReference(_record.BaseFileReference);
    }
}