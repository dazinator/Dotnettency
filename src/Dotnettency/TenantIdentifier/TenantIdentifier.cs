using System;

namespace Dotnettency
{
    public class TenantIdentifier : IEquatable<TenantIdentifier>
    {
        public TenantIdentifier(Uri key)
        {
            Uri = key;
        }

        public Uri Uri { get; set; }

        public static implicit operator TenantIdentifier(Uri key)
        {
            TenantIdentifier value = new TenantIdentifier(key);
            return value;
        }

        public bool Equals(TenantIdentifier other)
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
            return Equals(other as TenantIdentifier);
        }
    }
}
