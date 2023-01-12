using DiscUtils.Core.CoreCompat;
using DiscUtils.Ntfs;

namespace DiscUtils.FileSystems
{
    public static class SetupHelper
    {
        public static void SetupFileSystems()
        {
            Core.Setup.SetupHelper.RegisterAssembly(ReflectionHelper.GetAssembly(typeof(NtfsFileSystem)));
        }
    }
}