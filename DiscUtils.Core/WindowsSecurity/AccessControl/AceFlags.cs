using System;

namespace DiscUtils.Core.WindowsSecurity.AccessControl
{
    [Flags]
    public enum AceFlags : byte
    {
        None = 0,
        ObjectInherit = 0x01,
        ContainerInherit = 0x02,
        NoPropagateInherit = 0x04,
        InheritOnly = 0x08,
        InheritanceFlags = ObjectInherit | ContainerInherit | NoPropagateInherit | InheritOnly,
        Inherited = 0x10,
        SuccessfulAccess = 0x40,
        FailedAccess = 0x80,
        AuditFlags = SuccessfulAccess | FailedAccess,
    }
}