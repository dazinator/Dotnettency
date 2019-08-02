using DotNet.Cabinets;
using System;
using System.IO;

namespace Dotnettency.TenantFileSystem
{
    public class DelegateActionTenantFileSystemProviderFactory<TTenant> : ITenantFileSystemProviderFactory<TTenant>
        where TTenant : class
    {
        private readonly Action<TenantFileSystemBuilderContext<TTenant>> _configureRoot;
        private readonly string _basePath;

        public DelegateActionTenantFileSystemProviderFactory(
            string basePath,
            Action<TenantFileSystemBuilderContext<TTenant>> configureRoot)
        {
            _basePath = basePath;
            _configureRoot = configureRoot;
        }

        public string SubfolderName { get; set; }

        public string GetBasePath()
        {
            if (!string.IsNullOrWhiteSpace(SubfolderName))
            {
                return Path.Combine(_basePath, SubfolderName);
            }
            else
            {
                return _basePath;
            }
        }
        public ICabinet GetCabinet(TTenant tenant)
        {
            var defaultTenantsBaseFolderPath = GetBasePath();
            var builder = new TenantFileSystemBuilderContext<TTenant>(tenant, defaultTenantsBaseFolderPath);
            _configureRoot(builder);
            return builder.Build();
        }
    }
}
