using DotNet.Cabinets;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Dotnettency.HostingEnvironment
{
    public class DelegateTenantWebRootFileSystemProviderFactory<TTenant> : ITenantWebRootFileSystemProviderFatory<TTenant>
        where TTenant : class
    {
        private readonly Action<TenantFileSystemBuilderContext<TTenant>> _configureWebRoot;
        private readonly IHostingEnvironment _parentHostingEnvironment;

        public DelegateTenantWebRootFileSystemProviderFactory(
            IHostingEnvironment hostingEnv,
            Action<TenantFileSystemBuilderContext<TTenant>> configureWebRoot)
        {
            _parentHostingEnvironment = hostingEnv;
            _configureWebRoot = configureWebRoot;
        }

        public ICabinet GetWebRoot(TTenant tenant)
        {
            var defaultTenantsBaseFolderPath = Path.Combine(_parentHostingEnvironment.WebRootPath, ".tenants\\");
            var builder = new TenantFileSystemBuilderContext<TTenant>(tenant, defaultTenantsBaseFolderPath);
            _configureWebRoot(builder);
            return builder.Build();
        }
    }
}