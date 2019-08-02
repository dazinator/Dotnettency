using DotNet.Cabinets;
using System;

namespace Dotnettency.TenantFileSystem
{
    public static class TenantShellFileSystemExtensions
    {
        public static Lazy<ICabinet> GetOrAddTenantFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, string key, Lazy<ICabinet> factory)
            where TTenant : class
        {            
            return tenantShell.GetOrAddProperty(key, factory) as Lazy<ICabinet>;
        }

        public static Lazy<ICabinet> GetOrAddTenantFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, string key, Func<string, Lazy<ICabinet>> factory)
           where TTenant : class
        {
            return tenantShell.GetOrAddProperty(key, factory) as Lazy<ICabinet>;
        }

        public static Lazy<ICabinet> TryGetTenantFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, string key)
            where TTenant : class
        {
            tenantShell.TryGetProperty(key, out Lazy<ICabinet> result);
            return result;
        }

    }
}
