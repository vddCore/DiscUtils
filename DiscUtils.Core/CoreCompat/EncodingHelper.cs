using System.Text;

namespace DiscUtils.Core.CoreCompat
{
    internal static class EncodingHelper
    {
        private static bool _registered;

        public static void RegisterEncodings()
        {
            if (_registered)
                return;

            _registered = true;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}