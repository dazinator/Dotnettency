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
      
    }
}
