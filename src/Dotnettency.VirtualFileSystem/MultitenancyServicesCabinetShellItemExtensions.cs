using DotNet.Cabinets;
using Dotnettency.TenantFileSystem;
using Dotnettency.VirtualFileSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;

namespace Dotnettency
{
    public static partial class MultitenancyServicesCabinetShellItemExtensions
    {

        public const string ContentKey = "ContentRoot";
        public const string WebKey = "WebRoot";


        /// <summary>
        /// Configure the ICabinet that will be lazily initialised (asynchronously) on first consumption for a tenant, and stored in the <see cref="TenantShell{TTenant}"/> for the lifetime of the tenant. Inject <see cref="Task`<typeparamref name="IConfiguration"/>`" to access the lazy async value./>
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="configureItem"></param>
        /// <returns></returns>
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantFileSystem<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Func<TenantShellItemBuilderContext<TTenant>, ICabinet> configureItem)
             where TTenant : class
        {

            optionsBuilder.ConfigureTenantShellItem(configureItem);
            return optionsBuilder;
        }

        /// <summary>
        /// Configure the ICabinet that will be lazily initialised (asynchronously) on first consumption for a tenant, and stored in the <see cref="TenantShell{TTenant}"/> for the lifetime of the tenant. Inject <see cref="Task`<typeparamref name="ICabinet"/>`" to access the lazy async value./>
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="configureItem"></param>
        /// <returns></returns>
        /// <remarks>Support for PhysicalStorageCabinetBuilder</remarks>
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantFileSystem<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, string basePhysicalPath, Action<TenantShellItemBuilderContext<TTenant>, PhysicalStorageCabinetBuilder> configureItem)
             where TTenant : class
        {
            Func<TenantShellItemBuilderContext<TTenant>, ICabinet> factory = (c) =>
            {
                var builder = new PhysicalStorageCabinetBuilder(basePhysicalPath);
                configureItem(c, builder);
                return builder.Build();
            };

            optionsBuilder.ConfigureTenantShellItem(factory);
            return optionsBuilder;
        }

        public static NamedFileSystemOptionsBuilder<TTenant, ICabinet> AddFileSystem<TTenant>(this NamedTenantShellItemOptionsBuilder<TTenant, ICabinet> builder, string name, string basePhysicalPath, Action<TenantShellItemBuilderContext<TTenant>, PhysicalStorageCabinetBuilder> configureItem)
 where TTenant : class
        {
            Func<TenantShellItemBuilderContext<TTenant>, ICabinet> factory = (c) =>
            {
                var cabinetBuilder = new PhysicalStorageCabinetBuilder(basePhysicalPath);
                configureItem(c, cabinetBuilder);
                return cabinetBuilder.Build();
            };

            builder.Add(name, factory);
            return new NamedFileSystemOptionsBuilder<TTenant, ICabinet>(builder);
        }
        public static NamedFileSystemOptionsBuilder<TTenant, ICabinet> AddContentFileSystem<TTenant>(this NamedTenantShellItemOptionsBuilder<TTenant, ICabinet> builder, string basePhysicalPath, Action<TenantShellItemBuilderContext<TTenant>, PhysicalStorageCabinetBuilder> configureItem)
where TTenant : class
        {
            return builder.AddFileSystem<TTenant>(ContentKey, basePhysicalPath, configureItem);
        }

        public static NamedFileSystemOptionsBuilder<TTenant, ICabinet> AddWebFileSystem<TTenant>(this NamedTenantShellItemOptionsBuilder<TTenant, ICabinet> builder, string basePhysicalPath, Action<TenantShellItemBuilderContext<TTenant>, PhysicalStorageCabinetBuilder> configureItem)
where TTenant : class
        {
            return builder.AddFileSystem<TTenant>(WebKey, basePhysicalPath, configureItem);
        }

        /// <summary>
        /// Configure ICabinet (file systems) for a tenant, each with a given name, that will be lazily initialised (asynchronously) on first consumption, and stored in the <see cref="TenantShell{TTenant}"/> for the lifetime of the tenant. Inject <see cref="Task`<typeparamref name="Func`string, ICabinet`"/>`" to access the lazy async value./>
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="configureItem"></param>
        /// <returns></returns>
        public static MultitenancyOptionsBuilder<TTenant> ConfigureNamedTenantFileSystems<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Action<NamedTenantShellItemOptionsBuilder<TTenant, ICabinet>> configureItem)
             where TTenant : class
        {

            return optionsBuilder.ConfigureNamedTenantShellItems<TTenant, ICabinet>(configureItem);
        }

        public static MultitenancyOptionsBuilder<TTenant> UseVirtualFileSystemFileProvider<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Action<IFileProvider> useFileProvider)
             where TTenant : class
        {
            var contextProvider = optionsBuilder.HttpContextProvider;
            var multiTenantFileProvider = new MultitenantFileProvider<TTenant>(contextProvider);
            useFileProvider?.Invoke(multiTenantFileProvider);
            return optionsBuilder;
        }

        public static MultitenancyOptionsBuilder<TTenant> UseVirtualFileSystemFileProvider<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, string fileSystemName, Action<IFileProvider> useFileProvider)
            where TTenant : class
        {
            var contextProvider = optionsBuilder.HttpContextProvider;
            var multiTenantFileProvider = new MultitenantFileProvider<TTenant>(contextProvider, fileSystemName);
            useFileProvider?.Invoke(multiTenantFileProvider);
            return optionsBuilder;
        }

        public static MultitenancyOptionsBuilder<TTenant> UseContentVirtualFileSystemFileProvider<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Action<IFileProvider> useFileProvider)
    where TTenant : class
        {
            var contextProvider = optionsBuilder.HttpContextProvider;
            var multiTenantFileProvider = new MultitenantFileProvider<TTenant>(contextProvider, ContentKey);
            useFileProvider?.Invoke(multiTenantFileProvider);
            return optionsBuilder;
        }

        public static MultitenancyOptionsBuilder<TTenant> UseWebVirtualFileSystemFileProvider<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Action<IFileProvider> useFileProvider)
   where TTenant : class
        {
            var contextProvider = optionsBuilder.HttpContextProvider;
            var multiTenantFileProvider = new MultitenantFileProvider<TTenant>(contextProvider, WebKey);
            useFileProvider?.Invoke(multiTenantFileProvider);
            return optionsBuilder;
        }



    }
}
