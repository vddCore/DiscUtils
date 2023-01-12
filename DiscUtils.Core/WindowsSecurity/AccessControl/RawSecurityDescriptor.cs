using System;

namespace DiscUtils.Core.WindowsSecurity.AccessControl
{
    public sealed class RawSecurityDescriptor : GenericSecurityDescriptor
    {
        private ControlFlags _controlFlags;

        internal override GenericAcl InternalDacl => DiscretionaryAcl;
        internal override GenericAcl InternalSacl => SystemAcl;
        internal override byte InternalReservedField => ResourceManagerControl;
        
        public override ControlFlags ControlFlags => _controlFlags;

        public RawAcl DiscretionaryAcl { get; set; }
        public override SecurityIdentifier Group { get; set; }
        public override SecurityIdentifier Owner { get; set; }
        public byte ResourceManagerControl { get; set; }
        public RawAcl SystemAcl { get; set; }
        
        public RawSecurityDescriptor(string sddlForm)
        {
            if (sddlForm == null)
                throw new ArgumentNullException(nameof(sddlForm));

            ParseSddl(sddlForm.Replace(" ", ""));

            _controlFlags |= ControlFlags.SelfRelative;
        }

        public RawSecurityDescriptor(byte[] binaryForm, int offset)
        {
            if (binaryForm == null)
                throw new ArgumentNullException(nameof(binaryForm));

            if (offset < 0 || offset > binaryForm.Length - 0x14)
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset out of range");

            if (binaryForm[offset] != 1)
                throw new ArgumentException("Unrecognized Security Descriptor revision.", nameof(binaryForm));

            ResourceManagerControl = binaryForm[offset + 0x01];
            _controlFlags = (ControlFlags)ReadUShort(binaryForm, offset + 0x02);

            int ownerPos = ReadInt(binaryForm, offset + 0x04);
            int groupPos = ReadInt(binaryForm, offset + 0x08);
            int saclPos = ReadInt(binaryForm, offset + 0x0C);
            int daclPos = ReadInt(binaryForm, offset + 0x10);

            if (ownerPos != 0)
                Owner = new SecurityIdentifier(binaryForm, ownerPos);

            if (groupPos != 0)
                Group = new SecurityIdentifier(binaryForm, groupPos);

            if (saclPos != 0)
                SystemAcl = new RawAcl(binaryForm, saclPos);

            if (daclPos != 0)
                DiscretionaryAcl = new RawAcl(binaryForm, daclPos);
        }

        public RawSecurityDescriptor(ControlFlags flags,
                                     SecurityIdentifier owner,
                                     SecurityIdentifier group,
                                     RawAcl systemAcl,
                                     RawAcl discretionaryAcl)
        {
            _controlFlags = flags;
            Owner = owner;
            Group = group;
            SystemAcl = systemAcl;
            DiscretionaryAcl = discretionaryAcl;
        }

        public void SetFlags(ControlFlags flags)
        {
            _controlFlags = flags | ControlFlags.SelfRelative;
        }

        private void ParseSddl(string sddlForm)
        {
            ControlFlags flags = ControlFlags.None;

            int pos = 0;
            while (pos < sddlForm.Length - 2)
            {
                switch (sddlForm.Substring(pos, 2))
                {
                    case "O:":
                        pos += 2;
                        Owner = SecurityIdentifier.ParseSddlForm(sddlForm, ref pos);
                        break;

                    case "G:":
                        pos += 2;
                        Group = SecurityIdentifier.ParseSddlForm(sddlForm, ref pos);
                        break;

                    case "D:":
                        pos += 2;
                        DiscretionaryAcl = RawAcl.ParseSddlForm(sddlForm, true, ref flags, ref pos);
                        flags |= ControlFlags.DiscretionaryAclPresent;
                        break;

                    case "S:":
                        pos += 2;
                        SystemAcl = RawAcl.ParseSddlForm(sddlForm, false, ref flags, ref pos);
                        flags |= ControlFlags.SystemAclPresent;
                        break;
                    default:

                        throw new ArgumentException("Invalid SDDL.", nameof(sddlForm));
                }
            }

            if (pos != sddlForm.Length)
            {
                throw new ArgumentException("Invalid SDDL.", nameof(sddlForm));
            }

            SetFlags(flags);
        }

        private ushort ReadUShort(byte[] buffer, int offset)
        {
            return (ushort)((((int)buffer[offset + 0]) << 0)
                            | (((int)buffer[offset + 1]) << 8));
        }

        private int ReadInt(byte[] buffer, int offset)
        {
            return (((int)buffer[offset + 0]) << 0)
                   | (((int)buffer[offset + 1]) << 8)
                   | (((int)buffer[offset + 2]) << 16)
                   | (((int)buffer[offset + 3]) << 24);
        }
    }
}