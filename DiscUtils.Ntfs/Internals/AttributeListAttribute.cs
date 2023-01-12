using System.Collections.Generic;
using DiscUtils.Streams;
using DiscUtils.Streams.Util;

namespace DiscUtils.Ntfs.Internals
{
    /// <summary>
    /// List of attributes for files that are split over multiple Master File Table entries.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Files with lots of attribute data (for example that have become very fragmented) contain
    /// this attribute in their 'base' Master File Table entry.  This attribute acts as an index,
    /// indicating for each attribute in the file, which Master File Table entry contains the
    /// attribute.
    /// </para>
    /// </remarks>
    public sealed class AttributeListAttribute : GenericAttribute
    {
        private readonly AttributeList _list;

        internal AttributeListAttribute(INtfsContext context, AttributeRecord record)
            : base(context, record)
        {
            byte[] content = StreamUtilities.ReadAll(Content);
            _list = new AttributeList();
            _list.ReadFrom(content, 0);
        }

        /// <summary>
        /// Gets the entries in this attribute list.
        /// </summary>
        public ICollection<AttributeListEntry> Entries
        {
            get
            {
                List<AttributeListEntry> entries = new List<AttributeListEntry>();
                foreach (AttributeListRecord record in _list)
                {
                    entries.Add(new AttributeListEntry(record));
                }

                return entries;
            }
        }
    }
}