using DotNet.Cabinets;
using Dotnettency.TenantFileSystem;
using System;

namespace Dotnettency.TenantFileSystem
{
    public static class TenantShellHostingEnvironmentFileSystemExtensions
    {
        public const string ContentRootKey = nameof(TenantShellHostingEnvironmentFileSystemExtensions) + "-ContentRoot";
        public const string WebRootKey = nameof(TenantShellHostingEnvironmentFileSystemExtensions) + "-WebRoot";

        public static Lazy<ICabinet> GetOrAddTenantContentRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<ICabinet> factory)
            where TTenant : class
        {
            return tenantShell.GetOrAddTenantFileSystem(ContentRootKey, factory) as Lazy<ICabinet>;           
        }

        public static Lazy<ICabinet> TryGetContentRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell)
            where TTenant : class
        {
            return tenantShell.TryGetTenantFileSystem(ContentRootKey) as Lazy<ICabinet>;
        }

        public static Lazy<ICabinet> TryGetWebRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell)
            where TTenant : class
        {
            return tenantShell.TryGetTenantFileSystem(WebRootKey) as Lazy<ICabinet>;
        }

        public static Lazy<ICabinet> GetOrAddTenantWebRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<ICabinet> factory)
            where TTenant : class
        {
            return tenantShell.GetOrAddTenantFileSystem(WebRootKey, factory) as Lazy<ICabinet>;           
        }
    }
}
