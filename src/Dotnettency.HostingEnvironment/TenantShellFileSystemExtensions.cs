using System;
using DotNet.Cabinets;
using Microsoft.AspNetCore.Hosting;

namespace Dotnettency.HostingEnvironment
{
    public static class TenantShellFileSystemExtensions
    {

        private const string ConentRootKey = nameof(TenantShellFileSystemExtensions) + "-ContentRoot";
        private const string WebRootKey = nameof(TenantShellFileSystemExtensions) + "-WebRoot";

        //public static Lazy<IHostingEnvironment> GetOrAddTenantHostingEnvironment<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<ICabinet> factory)
        // where TTenant : class
        //{
        //    var result = tenantShell.Properties.GetOrAdd(ConentRootKey, factory) as Lazy<ICabinet>;
        //    return result;
        //}

        public static Lazy<ICabinet> GetOrAddTenantContentRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<ICabinet> factory)
            where TTenant : class
        {
            var result = tenantShell.Properties.GetOrAdd(ConentRootKey, factory) as Lazy<ICabinet>;
            return result;
        }

        public static Lazy<ICabinet> GetOrAddTenantWebRootFileSystem<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<ICabinet> factory)
           where TTenant : class
        {
            var result = tenantShell.Properties.GetOrAdd(WebRootKey, factory) as Lazy<ICabinet>;
            return result;
        }
    }
}
