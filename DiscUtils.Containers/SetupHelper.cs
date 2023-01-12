using DiscUtils.Core.CoreCompat;

namespace DiscUtils.Containers
{
    public static class SetupHelper
    {
        public static void SetupContainers()
        {
            Core.Setup.SetupHelper.RegisterAssembly(ReflectionHelper.GetAssembly(typeof(Vhd.Disk)));
        }
    }
}