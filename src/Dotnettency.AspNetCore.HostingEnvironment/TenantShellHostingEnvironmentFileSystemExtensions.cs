using DotNet.Cabinets;
using Dotnettency.TenantFileSystem;
using System;

namespace Dotnettency.AspNetCore.HostingEnvironment
{
    public static class TenantShellHostingEnvironmentFileSystemExtensions
    {
        private const string ContentRootKey = nameof(TenantShellHostingEnvironmentFileSystemExtensions) + "-ContentRoot";
        private const string WebRootKey = nameof(TenantShellHostingEnvironmentFileSystemExtensions) + "-WebRoot";

        public static Lazy<ICabinet> GetOrAddTenantContentRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<ICabinet> factory)
            where TTenant : class
        {
            return tenantShell.GetOrAddTenantFileSystem(ContentRootKey, factory) as Lazy<ICabinet>;           
        }

        public static Lazy<ICabinet> GetOrAddTenantWebRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<ICabinet> factory)
            where TTenant : class
        {
            return tenantShell.GetOrAddTenantFileSystem(WebRootKey, factory) as Lazy<ICabinet>;           
        }
    }
}
