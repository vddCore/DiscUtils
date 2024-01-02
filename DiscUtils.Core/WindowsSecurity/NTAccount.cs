using System;

namespace DiscUtils.Core.WindowsSecurity
{
    public sealed class NTAccount : IdentityReference
    {
        public override string Value { get; }

        public NTAccount(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name.Length == 0)
                throw new ArgumentException("empty", nameof(name));
            Value = name;
        }

        public NTAccount(string domainName, string accountName)
        {
            if (accountName == null)
                throw new ArgumentNullException(nameof(accountName));
            if (accountName.Length == 0)
                throw new ArgumentException("empty", nameof(accountName));
            if (domainName == null)
                Value = accountName;
            else
                Value = domainName + "/" + accountName;
        }


        public override bool Equals(object o)
        {
            NTAccount nt = (o as NTAccount);
            if (nt == null)
                return false;
            return (nt.Value == Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool IsValidTargetType(Type targetType)
        {
            if (targetType == typeof(NTAccount))
                return true;
            if (targetType == typeof(SecurityIdentifier))
                return true;
            return false;
        }

        public override string ToString()
        {
            return Value;
        }

        public override IdentityReference Translate(Type targetType)
        {
            if (targetType == typeof(NTAccount))
                return this; // ? copy

            if (targetType == typeof(SecurityIdentifier))
            {
                WellKnownAccount acct = WellKnownAccount.LookupByName(this.Value);
                if (acct?.Sid == null)
                    throw new Exception("Cannot map account name: " + this.Value);

                return new SecurityIdentifier(acct.Sid);
            }

            throw new ArgumentException("Unknown type", nameof(targetType));
        }

        public static bool operator ==(NTAccount left, NTAccount right)
        {
            if (((object)left) == null)
                return (((object)right) == null);
            if (((object)right) == null)
                return false;
            return (left.Value == right.Value);
        }

        public static bool operator !=(NTAccount left, NTAccount right)
        {
            if (((object)left) == null)
                return (((object)right) != null);
            if (((object)right) == null)
                return true;
            return (left.Value != right.Value);
        }
    }
}