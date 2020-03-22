using System;

namespace Dotnettency
{
    public static class TenantIdentifierExtensions
    {
        public static bool TryGetMappedTenantKey<TKey>(this TenantIdentifier identifier, out TKey value)
        {
            var keyString = identifier.Uri.PathAndQuery;
            if(NoMappedTenantKey(keyString))
            {
                value = default(TKey);
                return false;
            }

            value = (TKey)Convert.ChangeType(keyString.Substring(1), typeof(TKey));
            return true;
        }

        private static bool NoMappedTenantKey(string keyString)
        {
            return string.IsNullOrWhiteSpace(keyString) || keyString == "/";
        }

        public static TenantIdentifier ToTenantIdentifier<TKey>(this TKey key)
        {
            return new TenantIdentifier(new System.Uri($"key://{typeof(TKey).Name}/{key}"));
        }
    }
}