using System;

namespace DiscUtils.Core.WindowsSecurity.AccessControl
{
    public abstract class QualifiedAce : KnownAce
    {
        private byte[] _opaque;

        internal QualifiedAce(AceType type, AceFlags flags, byte[] opaque)
            : base(type, flags)
        {
            SetOpaque(opaque);
        }

        internal QualifiedAce(byte[] binaryForm, int offset)
            : base(binaryForm, offset) { }

        public AceQualifier AceQualifier
        {
            get
            {
                switch (AceType)
                {
                    case AceType.AccessAllowed:
                    case AceType.AccessAllowedCallback:
                    case AceType.AccessAllowedCallbackObject:
                    case AceType.AccessAllowedCompound:
                    case AceType.AccessAllowedObject:
                        return AceQualifier.AccessAllowed;

                    case AceType.AccessDenied:
                    case AceType.AccessDeniedCallback:
                    case AceType.AccessDeniedCallbackObject:
                    case AceType.AccessDeniedObject:
                        return AceQualifier.AccessDenied;

                    case AceType.SystemAlarm:
                    case AceType.SystemAlarmCallback:
                    case AceType.SystemAlarmCallbackObject:
                    case AceType.SystemAlarmObject:
                        return AceQualifier.SystemAlarm;

                    case AceType.SystemAudit:
                    case AceType.SystemAuditCallback:
                    case AceType.SystemAuditCallbackObject:
                    case AceType.SystemAuditObject:
                        return AceQualifier.SystemAudit;

                    default:
                        throw new ArgumentException("Unrecognized ACE type: " + AceType);
                }
            }
        }

        public bool IsCallback =>
            AceType == AceType.AccessAllowedCallback
            || AceType == AceType.AccessAllowedCallbackObject
            || AceType == AceType.AccessDeniedCallback
            || AceType == AceType.AccessDeniedCallbackObject
            || AceType == AceType.SystemAlarmCallback
            || AceType == AceType.SystemAlarmCallbackObject
            || AceType == AceType.SystemAuditCallback
            || AceType == AceType.SystemAuditCallbackObject;

        public int OpaqueLength
        {
            get
            {
                if (_opaque == null)
                    return 0;
                return _opaque.Length;
            }
        }

        public byte[] GetOpaque()
        {
            return (byte[])_opaque?.Clone();
        }

        public void SetOpaque(byte[] opaque)
        {
            if (opaque == null)
                _opaque = null;
            else
                _opaque = (byte[])opaque.Clone();
        }
    }
}