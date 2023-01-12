namespace DiscUtils.Ntfs.Internals
{
    internal sealed class UnknownAttribute : GenericAttribute
    {
        public UnknownAttribute(INtfsContext context, AttributeRecord record)
            : base(context, record) {}
    }
}