using DotNet.Cabinets;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace Dotnettency.HostingEnvironment
{
    public class DelegateTenantContentRootFileSystemProviderFactory<TTenant> : ITenantContentRootFileSystemProviderFatory<TTenant>
        where TTenant : class
    {
        private readonly Action<TenantFileSystemBuilderContext<TTenant>> _configureContentRoot;
        private readonly IHostingEnvironment _parentHostingEnvironment;

        public DelegateTenantContentRootFileSystemProviderFactory(
            IHostingEnvironment hostingEnv,
            Action<TenantFileSystemBuilderContext<TTenant>> configureContentRoot)
        {
            _parentHostingEnvironment = hostingEnv;
            _configureContentRoot = configureContentRoot;
        }

        public ICabinet GetContentRoot(TTenant tenant)
        {
            var defaultTenantsBaseFolderPath = Path.Combine(_parentHostingEnvironment.ContentRootPath, ".tenants\\");
            var builder = new TenantFileSystemBuilderContext<TTenant>(tenant, defaultTenantsBaseFolderPath);

            _configureContentRoot(builder);
            return builder.Build();
        }
    }
}
