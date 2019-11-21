using DotNet.Cabinets;
using Dotnettency.VirtualFileSystem;
using System;
using System.IO;

namespace Dotnettency.TenantFileSystem
{
    public class DelegateActionTenantFileSystemProviderFactory<TTenant> : ITenantFileSystemProviderFactory<TTenant>
        where TTenant : class
    {
        private readonly Action<TenantShellItemBuilderContext<TTenant>, PhysicalStorageCabinetBuilder> _configureRoot;
        private readonly string _basePath;

        public DelegateActionTenantFileSystemProviderFactory(
            string basePath,
            Action<TenantShellItemBuilderContext<TTenant>, PhysicalStorageCabinetBuilder> configureRoot)
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
            var builder = new PhysicalStorageCabinetBuilder(defaultTenantsBaseFolderPath);
            var context = new TenantShellItemBuilderContext<TTenant>()
            {
                Services = null,
                Tenant = tenant
            };
            _configureRoot(context, builder);
            return builder.Build();
        }
    }
}
