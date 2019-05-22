using DotNet.Cabinets;
using System;

namespace Dotnettency.TenantFileSystem
{
    public static class TenantShellFileSystemExtensions
    {
        public static Lazy<ICabinet> GetOrAddTenantFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, string key, Lazy<ICabinet> factory)
            where TTenant : class
        {
            return tenantShell.Properties.GetOrAdd(key, factory) as Lazy<ICabinet>;
        }

        public static Lazy<ICabinet> TryGetTenantFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, string key)
            where TTenant : class
        {
            object result;
            var success = tenantShell.Properties.TryGetValue(key, out result);
            return result as Lazy<ICabinet>;
        }

    }
}
