using System;

namespace DiscUtils.Core.WindowsSecurity.AccessControl
{
    [Flags]
    public enum ControlFlags
    {
        None = 0x0000,
        OwnerDefaulted = 0x0001,
        GroupDefaulted = 0x0002,
        DiscretionaryAclPresent = 0x0004,
        DiscretionaryAclDefaulted = 0x0008,
        SystemAclPresent = 0x0010,
        SystemAclDefaulted = 0x0020,
        DiscretionaryAclUntrusted = 0x0040,
        ServerSecurity = 0x0080,
        DiscretionaryAclAutoInheritRequired = 0x0100,
        SystemAclAutoInheritRequired = 0x0200,
        DiscretionaryAclAutoInherited = 0x0400,
        SystemAclAutoInherited = 0x0800,
        DiscretionaryAclProtected = 0x1000,
        SystemAclProtected = 0x2000,
        RMControlValid = 0x4000,
        SelfRelative = 0x8000,
    }
}