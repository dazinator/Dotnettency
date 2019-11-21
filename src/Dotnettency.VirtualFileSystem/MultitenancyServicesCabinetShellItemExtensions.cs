using DotNet.Cabinets;
using Dotnettency.VirtualFileSystem;
using Microsoft.Extensions.Configuration;
using System;

namespace Dotnettency
{
    public static class MultitenancyServicesCabinetShellItemExtensions
    {
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

            optionsBuilder.ConfigureTenantShellItem<TTenant, ICabinet>(configureItem);
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

    }
}
