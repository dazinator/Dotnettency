using System;

namespace Dotnettency
{
    public class TenantDistinguisher : IEquatable<TenantDistinguisher>
    {
        public TenantDistinguisher(Uri key)
        {
            Uri = key;
        }

        public Uri Uri { get; set; }

        public static implicit operator TenantDistinguisher(Uri key)
        {
            TenantDistinguisher value = new TenantDistinguisher(key);
            return value;
        }

        public bool Equals(TenantDistinguisher other)
        {
            return other != null && other.Uri == Uri;
        }

        public override int GetHashCode()
        {
            if (Uri == null)
            {
                return 0;
            }
            return Uri.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as TenantDistinguisher);
        }
    }
}
