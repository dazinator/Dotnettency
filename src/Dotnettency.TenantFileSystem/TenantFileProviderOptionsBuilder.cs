using Microsoft.Extensions.FileProviders;
using System;

namespace Dotnettency.TenantFileSystem
{
    public class TenantFileProviderOptionsBuilder<TTenant>
    where TTenant : class
    {
        /// <summary>
        /// Default subfolder where tenant folders will be created for per tenant files.
        /// We default to a directory that starts with a "." as this prevents this folder from
        /// being navigable from any file providers that have access to parent diretory, forcing the user
        /// of a tenant's IFileProvider to access files in the tenants directory.
        /// </summary>
        public const string DefaultSubFolderName = ".tenants\\";

        public TenantFileProviderOptionsBuilder(
           MultitenancyOptionsBuilder<TTenant> builder)
        {
            Builder = builder;
        }

        public MultitenancyOptionsBuilder<TTenant> Builder { get; set; }


        public MultitenancyOptionsBuilder<TTenant> ConfigureTenantContentFileProvider(
            string basePath,
            Action<TenantFileSystemBuilderContext<TTenant>> configureRoot,
            Action<IFileProvider> useFileProvider)
        {
            return ConfigureTenantFileProvider(TenantShellHostingEnvironmentFileSystemExtensions.ContentRootKey, basePath, configureRoot, useFileProvider);

        }

        public MultitenancyOptionsBuilder<TTenant> ConfigureTenantWebRootFileProvider(
            string basePath,
            Action<TenantFileSystemBuilderContext<TTenant>> configureRoot,
            Action<IFileProvider> useFileProvider)
        {
            return ConfigureTenantFileProvider(TenantShellHostingEnvironmentFileSystemExtensions.WebRootKey, basePath, configureRoot, useFileProvider);
        }

        public MultitenancyOptionsBuilder<TTenant> ConfigureTenantFileProvider(
            string key,
            string basePath,
           Action<TenantFileSystemBuilderContext<TTenant>> configureRoot,
           Action<IFileProvider> useFileProvider,
           string subFolderName = DefaultSubFolderName)
        {
            var factory = new DelegateTenantFileSystemProviderFactory<TTenant>(basePath, configureRoot);
            factory.SubfolderName = subFolderName;
            var contextProvider = this.Builder.HttpContextProvider;

            var multiTenantFileProvider = new CurrentTenantFileProvider<TTenant>(contextProvider, factory, key);
            useFileProvider?.Invoke(multiTenantFileProvider);

            return Builder;
        }
    }
}
