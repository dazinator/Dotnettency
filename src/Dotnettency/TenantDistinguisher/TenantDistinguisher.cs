using System;

namespace Dotnettency
{
    public class TenantDistinguisher : IEquatable<TenantDistinguisher>
    {
        public TenantDistinguisher(string key)
        {
            Key = key;
        }

        public string Key { get; set; }

        public static implicit operator TenantDistinguisher(string key)
        {
            TenantDistinguisher value = new TenantDistinguisher(key);
            return value;
        }


        public override int GetHashCode()
        {
            if (Key == null)
            {
                return 0;
            }
            return Key.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as TenantDistinguisher);
        }

        public bool Equals(TenantDistinguisher other)
        {
            return other != null && other.Key == Key;
        }
    }




}
