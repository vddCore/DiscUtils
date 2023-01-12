using System.Globalization;
using System.Text;

namespace DiscUtils.Core.WindowsSecurity.AccessControl
{
    public abstract class KnownAce : GenericAce
    {
        public int AccessMask { get; set; }
        public SecurityIdentifier SecurityIdentifier { get; set; }
        
        internal KnownAce(AceType type, AceFlags flags)
            : base(type, flags) { }

        internal KnownAce(byte[] binaryForm, int offset)
            : base(binaryForm, offset) { }

        internal static string GetSddlAccessRights(int accessMask)
        {
            string ret = GetSddlAliasRights(accessMask);
            if (!string.IsNullOrEmpty(ret))
                return ret;

            return string.Format(CultureInfo.InvariantCulture,
                "0x{0:x}", accessMask);
        }

        private static string GetSddlAliasRights(int accessMask)
        {
            SddlAccessRight[] rights = SddlAccessRight.Decompose(accessMask);
            if (rights == null)
                return null;

            StringBuilder ret = new StringBuilder();
            foreach (var right in rights)
            {
                ret.Append(right.Name);
            }

            return ret.ToString();
        }
    }
}