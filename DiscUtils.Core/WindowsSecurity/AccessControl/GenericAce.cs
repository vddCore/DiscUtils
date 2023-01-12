using System;
using System.Globalization;
using System.Text;

namespace DiscUtils.Core.WindowsSecurity.AccessControl
{
    public abstract class GenericAce
    {
        private AceFlags _aceFlags;
        
        public AceFlags AceFlags
        {
            get => _aceFlags;
            set => _aceFlags = value;
        }

        public AceType AceType { get; }

        public AuditFlags AuditFlags
        {
            get
            {
                AuditFlags ret = AuditFlags.None;
                if ((_aceFlags & AceFlags.SuccessfulAccess) != 0)
                    ret |= AuditFlags.Success;
                if ((_aceFlags & AceFlags.FailedAccess) != 0)
                    ret |= AuditFlags.Failure;
                return ret;
            }
        }

        public abstract int BinaryLength { get; }

        public InheritanceFlags InheritanceFlags
        {
            get
            {
                InheritanceFlags ret = InheritanceFlags.None;
                if ((_aceFlags & AceFlags.ObjectInherit) != 0)
                    ret |= InheritanceFlags.ObjectInherit;
                if ((_aceFlags & AceFlags.ContainerInherit) != 0)
                    ret |= InheritanceFlags.ContainerInherit;
                return ret;
            }
        }

        public bool IsInherited => (_aceFlags & AceFlags.Inherited) != AceFlags.None;

        public PropagationFlags PropagationFlags
        {
            get
            {
                PropagationFlags ret = PropagationFlags.None;
                if ((_aceFlags & AceFlags.InheritOnly) != 0)
                    ret |= PropagationFlags.InheritOnly;
                if ((_aceFlags & AceFlags.NoPropagateInherit) != 0)
                    ret |= PropagationFlags.NoPropagateInherit;
                return ret;
            }
        }

        internal GenericAce(AceType type, AceFlags flags)
        {
            if (type > AceType.MaxDefinedAceType)
            {
                throw new ArgumentOutOfRangeException(nameof(type));
            }

            AceType = type;
            _aceFlags = flags;
        }

        internal GenericAce(byte[] binaryForm, int offset)
        {
            if (binaryForm == null)
                throw new ArgumentNullException(nameof(binaryForm));

            if (offset < 0 || offset > binaryForm.Length - 2)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset out of range");

            AceType = (AceType)binaryForm[offset];
            _aceFlags = (AceFlags)binaryForm[offset + 1];
        }

        public GenericAce Copy()
        {
            byte[] buffer = new byte[BinaryLength];
            GetBinaryForm(buffer, 0);
            return CreateFromBinaryForm(buffer, 0);
        }

        public static GenericAce CreateFromBinaryForm(byte[] binaryForm, int offset)
        {
            if (binaryForm == null)
                throw new ArgumentNullException(nameof(binaryForm));

            if (offset < 0 || offset > binaryForm.Length - 1)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset out of range");

            AceType type = (AceType)binaryForm[offset];
            if (IsObjectType(type))
                return new ObjectAce(binaryForm, offset);
            else
                return new CommonAce(binaryForm, offset);
        }

        public sealed override bool Equals(object o)
        {
            return this == (o as GenericAce);
        }

        public abstract void GetBinaryForm(byte[] binaryForm, int offset);

        public sealed override int GetHashCode()
        {
            byte[] buffer = new byte[BinaryLength];
            GetBinaryForm(buffer, 0);

            int code = 0;
            for (int i = 0; i < buffer.Length; ++i)
            {
                code = (code << 3) | ((code >> 29) & 0x7);
                code ^= ((int)buffer[i]) & 0xff;
            }

            return code;
        }

        public static bool operator ==(GenericAce left, GenericAce right)
        {
            if (((object)left) == null)
                return ((object)right) == null;

            if (((object)right) == null)
                return false;

            int leftLen = left.BinaryLength;
            int rightLen = right.BinaryLength;
            if (leftLen != rightLen)
                return false;

            byte[] leftBuffer = new byte[leftLen];
            byte[] rightBuffer = new byte[rightLen];
            left.GetBinaryForm(leftBuffer, 0);
            right.GetBinaryForm(rightBuffer, 0);

            for (int i = 0; i < leftLen; ++i)
            {
                if (leftBuffer[i] != rightBuffer[i])
                    return false;
            }

            return true;
        }

        public static bool operator !=(GenericAce left, GenericAce right)
        {
            if (((object)left) == null)
                return ((object)right) != null;

            if (((object)right) == null)
                return true;

            int leftLen = left.BinaryLength;
            int rightLen = right.BinaryLength;
            if (leftLen != rightLen)
                return true;

            byte[] leftBuffer = new byte[leftLen];
            byte[] rightBuffer = new byte[rightLen];
            left.GetBinaryForm(leftBuffer, 0);
            right.GetBinaryForm(rightBuffer, 0);

            for (int i = 0; i < leftLen; ++i)
            {
                if (leftBuffer[i] != rightBuffer[i])
                    return true;
            }

            return false;
        }

        internal abstract string GetSddlForm();

        internal static GenericAce CreateFromSddlForm(string sddlForm, ref int pos)
        {
            if (sddlForm[pos] != '(')
                throw new ArgumentException("Invalid SDDL string.", nameof(sddlForm));

            int endPos = sddlForm.IndexOf(')', pos);
            if (endPos < 0)
                throw new ArgumentException("Invalid SDDL string.", nameof(sddlForm));

            int count = endPos - (pos + 1);
            string elementsStr = sddlForm.Substring(pos + 1,
                count);
            elementsStr = elementsStr.ToUpperInvariant();
            string[] elements = elementsStr.Split(';');
            if (elements.Length != 6)
                throw new ArgumentException("Invalid SDDL string.", nameof(sddlForm));

            ObjectAceFlags objFlags = ObjectAceFlags.None;

            AceType type = ParseSddlAceType(elements[0]);

            AceFlags flags = ParseSddlAceFlags(elements[1]);

            int accessMask = ParseSddlAccessRights(elements[2]);

            Guid objectType = Guid.Empty;
            if (!string.IsNullOrEmpty(elements[3]))
            {
                objectType = new Guid(elements[3]);
                objFlags |= ObjectAceFlags.ObjectAceTypePresent;
            }

            Guid inhObjectType = Guid.Empty;
            if (!string.IsNullOrEmpty(elements[4]))
            {
                inhObjectType = new Guid(elements[4]);
                objFlags |= ObjectAceFlags.InheritedObjectAceTypePresent;
            }

            SecurityIdentifier sid
                = new SecurityIdentifier(elements[5]);

            if (type == AceType.AccessAllowedCallback
                || type == AceType.AccessDeniedCallback)
                throw new NotImplementedException("Conditional ACEs not supported");

            pos = endPos + 1;

            if (IsObjectType(type))
                return new ObjectAce(type, flags, accessMask, sid, objFlags, objectType, inhObjectType, null);
            else
            {
                if (objFlags != ObjectAceFlags.None)
                    throw new ArgumentException("Invalid SDDL string.", nameof(sddlForm));
                return new CommonAce(type, flags, accessMask, sid, null);
            }
        }

        private static bool IsObjectType(AceType type)
        {
            return type == AceType.AccessAllowedCallbackObject
                   || type == AceType.AccessAllowedObject
                   || type == AceType.AccessDeniedCallbackObject
                   || type == AceType.AccessDeniedObject
                   || type == AceType.SystemAlarmCallbackObject
                   || type == AceType.SystemAlarmObject
                   || type == AceType.SystemAuditCallbackObject
                   || type == AceType.SystemAuditObject;
        }

        internal static string GetSddlAceType(AceType type)
        {
            switch (type)
            {
                case AceType.AccessAllowed:
                    return "A";
                case AceType.AccessDenied:
                    return "D";
                case AceType.AccessAllowedObject:
                    return "OA";
                case AceType.AccessDeniedObject:
                    return "OD";
                case AceType.SystemAudit:
                    return "AU";
                case AceType.SystemAlarm:
                    return "AL";
                case AceType.SystemAuditObject:
                    return "OU";
                case AceType.SystemAlarmObject:
                    return "OL";
                case AceType.AccessAllowedCallback:
                    return "XA";
                case AceType.AccessDeniedCallback:
                    return "XD";
                default:
                    throw new ArgumentException("Unable to convert to SDDL ACE type: " + type, nameof(type));
            }
        }

        private static AceType ParseSddlAceType(string type)
        {
            switch (type)
            {
                case "A":
                    return AceType.AccessAllowed;
                case "D":
                    return AceType.AccessDenied;
                case "OA":
                    return AceType.AccessAllowedObject;
                case "OD":
                    return AceType.AccessDeniedObject;
                case "AU":
                    return AceType.SystemAudit;
                case "AL":
                    return AceType.SystemAlarm;
                case "OU":
                    return AceType.SystemAuditObject;
                case "OL":
                    return AceType.SystemAlarmObject;
                case "XA":
                    return AceType.AccessAllowedCallback;
                case "XD":
                    return AceType.AccessDeniedCallback;
                default:
                    throw new ArgumentException("Unable to convert SDDL to ACE type: " + type, nameof(type));
            }
        }

        internal static string GetSddlAceFlags(AceFlags flags)
        {
            StringBuilder result = new StringBuilder();
            if ((flags & AceFlags.ObjectInherit) != 0)
                result.Append("OI");
            if ((flags & AceFlags.ContainerInherit) != 0)
                result.Append("CI");
            if ((flags & AceFlags.NoPropagateInherit) != 0)
                result.Append("NP");
            if ((flags & AceFlags.InheritOnly) != 0)
                result.Append("IO");
            if ((flags & AceFlags.Inherited) != 0)
                result.Append("ID");
            if ((flags & AceFlags.SuccessfulAccess) != 0)
                result.Append("SA");
            if ((flags & AceFlags.FailedAccess) != 0)
                result.Append("FA");
            return result.ToString();
        }

        private static AceFlags ParseSddlAceFlags(string flags)
        {
            AceFlags ret = AceFlags.None;

            int pos = 0;
            while (pos < flags.Length - 1)
            {
                string flag = flags.Substring(pos, 2);
                switch (flag)
                {
                    case "CI":
                        ret |= AceFlags.ContainerInherit;
                        break;
                    case "OI":
                        ret |= AceFlags.ObjectInherit;
                        break;
                    case "NP":
                        ret |= AceFlags.NoPropagateInherit;
                        break;
                    case "IO":
                        ret |= AceFlags.InheritOnly;
                        break;
                    case "ID":
                        ret |= AceFlags.Inherited;
                        break;
                    case "SA":
                        ret |= AceFlags.SuccessfulAccess;
                        break;
                    case "FA":
                        ret |= AceFlags.FailedAccess;
                        break;
                    default:
                        throw new ArgumentException("Invalid SDDL string.", nameof(flags));
                }

                pos += 2;
            }

            if (pos != flags.Length)
                throw new ArgumentException("Invalid SDDL string.", nameof(flags));

            return ret;
        }

        private static int ParseSddlAccessRights(string accessMask)
        {
            if (accessMask.StartsWith("0X"))
            {
                return int.Parse(accessMask.Substring(2),
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
            }
            else if (Char.IsDigit(accessMask, 0))
            {
                return int.Parse(accessMask,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture);
            }
            else
            {
                return ParseSddlAliasRights(accessMask);
            }
        }

        private static int ParseSddlAliasRights(string accessMask)
        {
            int ret = 0;

            int pos = 0;
            while (pos < accessMask.Length - 1)
            {
                string flag = accessMask.Substring(pos, 2);
                SddlAccessRight right = SddlAccessRight.LookupByName(flag);
                if (right == null)
                    throw new ArgumentException("Invalid SDDL string.", nameof(accessMask));

                ret |= right.Value;
                pos += 2;
            }

            if (pos != accessMask.Length)
                throw new ArgumentException("Invalid SDDL string.", nameof(accessMask));

            return ret;
        }

        internal static ushort ReadUShort(byte[] buffer, int offset)
        {
            return (ushort)((((int)buffer[offset + 0]) << 0)
                            | (((int)buffer[offset + 1]) << 8));
        }

        internal static int ReadInt(byte[] buffer, int offset)
        {
            return (((int)buffer[offset + 0]) << 0)
                   | (((int)buffer[offset + 1]) << 8)
                   | (((int)buffer[offset + 2]) << 16)
                   | (((int)buffer[offset + 3]) << 24);
        }

        internal static void WriteInt(int val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)val;
            buffer[offset + 1] = (byte)(val >> 8);
            buffer[offset + 2] = (byte)(val >> 16);
            buffer[offset + 3] = (byte)(val >> 24);
        }

        internal static void WriteUShort(ushort val, byte[] buffer,
                                         int offset)
        {
            buffer[offset] = (byte)val;
            buffer[offset + 1] = (byte)(val >> 8);
        }
    }
}