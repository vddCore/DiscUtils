using System.IO;

namespace DiscUtils.Ntfs
{
    internal sealed class NtfsContext : INtfsContext
    {
        public Stream RawStream { get; set; }

        public AttributeDefinitions AttributeDefinitions { get; set; }

        public UpperCase UpperCase { get; set; }

        public BiosParameterBlock BiosParameterBlock { get; set; }

        public MasterFileTable Mft { get; set; }

        public ClusterBitmap ClusterBitmap { get; set; }

        public SecurityDescriptors SecurityDescriptors { get; set; }

        public ObjectIds ObjectIds { get; set; }

        public ReparsePoints ReparsePoints { get; set; }

        public Quotas Quotas { get; set; }

        public NtfsOptions Options { get; set; }

        public GetFileByIndexFn GetFileByIndex { get; set; }

        public GetFileByRefFn GetFileByRef { get; set; }

        public GetDirectoryByIndexFn GetDirectoryByIndex { get; set; }

        public GetDirectoryByRefFn GetDirectoryByRef { get; set; }

        public AllocateFileFn AllocateFile { get; set; }

        public ForgetFileFn ForgetFile { get; set; }

        public bool ReadOnly { get; set; }
    }
}