using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class MultitenancyServicesConfigurationShellItemExtensions
    {
        /// <summary>
        /// Configure the IConfiguration that will be lazily initialised (asynchronously) on first consumption for a tenant, and stored in the <see cref="TenantShell{TTenant}"/> for the lifetime of the tenant. Inject <see cref="Task`<typeparamref name="IConfiguration"/>`" to access the lazy async value./>
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="configureItem"></param>
        /// <returns></returns>
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantConfiguration<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Func<TenantShellItemBuilderContext<TTenant>, IConfiguration> configureItem)
             where TTenant : class
        {

            optionsBuilder.ConfigureTenantShellItem<TTenant, IConfiguration>(configureItem);
            return optionsBuilder;
        }

        /// <summary>
        /// Configure the IConfiguration that will be lazily initialised (asynchronously) on first consumption for a tenant, and stored in the <see cref="TenantShell{TTenant}"/> for the lifetime of the tenant. Inject <see cref="Task`<typeparamref name="IConfiguration"/>`" to access the lazy async value./>
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="configureItem"></param>
        /// <returns></returns>
        /// <remarks>Support for IConfigurationBuilder</remarks>
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantConfiguration<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder,
            Action<TenantShellItemBuilderContext<TTenant>, IConfigurationBuilder> configureItem)
             where TTenant : class
        {
            Func<TenantShellItemBuilderContext<TTenant>, IConfiguration> factory = (c) =>
            {
                var builder = new ConfigurationBuilder();
                configureItem(c, builder);
                return builder.Build();
            };

            optionsBuilder.ConfigureTenantShellItem<TTenant, IConfiguration>(factory);
            return optionsBuilder;
        }

        /// <summary>
        /// Configure the IConfiguration that will be lazily initialised (asynchronously) on first consumption for a tenant, and stored in the <see cref="TenantShell{TTenant}"/> for the lifetime of the tenant. Inject <see cref="Task`<typeparamref name="IConfiguration"/>`" to access the lazy async value./>
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="configureItem"></param>
        /// <returns></returns>
        /// <remarks>Support for IConfigurationBuilder</remarks>
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantConfigurationAsync<TTenant>(
            this MultitenancyOptionsBuilder<TTenant> optionsBuilder,
            Func<TenantShellItemBuilderContext<TTenant>, IConfigurationBuilder, Task> configureItem)
             where TTenant : class
        {
            Func<TenantShellItemBuilderContext<TTenant>, Task<IConfiguration>> factory = async (c) =>
            {
                var builder = new ConfigurationBuilder();
                await configureItem(c, builder);
                return builder.Build();
            };

            optionsBuilder.ConfigureTenantShellItem<TTenant, Task<IConfiguration>>(factory);
            return optionsBuilder;
        }

    }  
}
