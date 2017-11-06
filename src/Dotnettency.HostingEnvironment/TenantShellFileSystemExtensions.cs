using DotNet.Cabinets;
using System;

namespace Dotnettency.HostingEnvironment
{
    public static class TenantShellFileSystemExtensions
    {
        private const string ContentRootKey = nameof(TenantShellFileSystemExtensions) + "-ContentRoot";
        private const string WebRootKey = nameof(TenantShellFileSystemExtensions) + "-WebRoot";

        public static Lazy<ICabinet> GetOrAddTenantContentRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<ICabinet> factory)
            where TTenant : class
        {
            return tenantShell.Properties.GetOrAdd(ContentRootKey, factory) as Lazy<ICabinet>;
        }

        public static Lazy<ICabinet> GetOrAddTenantWebRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<ICabinet> factory)
            where TTenant : class
        {
            return tenantShell.Properties.GetOrAdd(WebRootKey, factory) as Lazy<ICabinet>;
        }
    }
}
