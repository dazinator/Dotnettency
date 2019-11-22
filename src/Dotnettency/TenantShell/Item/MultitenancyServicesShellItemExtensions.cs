using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class MultitenancyServicesShellItemExtensions
    {
        /// <summary>
        /// Configure an item of type <see cref="TItem"/> that will be lazily initialised (asynchronously) on first consumption for a tenant and stored in the <see cref="TenantShell{TTenant}"/> for the lifetime of the tenant. If <see cref="TItem"/> implements <see cref="IDisposable"/> then it will be disposed of when the tenant is restarted.
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <typeparam name="TTItem"></typeparam>
        /// <param name="optionsBuilder"></param>
        /// <param name="configureItem"></param>
        /// <returns></returns>
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantShellItem<TTenant, TTItem>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Func<TenantShellItemBuilderContext<TTenant>, TTItem> configureItem)
             where TTenant : class
        {
            optionsBuilder.Services.ConfigureTenantShellItem(configureItem);
            return optionsBuilder;
        }

        public static MultitenancyOptionsBuilder<TTenant> ConfigureNamedTenantShellItems<TTenant, TTItem>(this MultitenancyOptionsBuilder<TTenant> builder, Action<NamedTenantShellItemOptionsBuilder<TTenant, TTItem>> configure)
where TTenant : class
        {
            var namedBuilder = new NamedTenantShellItemOptionsBuilder<TTenant, TTItem>(builder);
            configure(namedBuilder);
            namedBuilder.Build();
            return builder;
        }


        //        public static MultitenancyOptionsBuilder<TTenant> ConfigureNamedTenantShellItems<TTenant, TTItem>(this MultitenancyOptionsBuilder<TTenant> builder, Action<NamedTenantShellItemOptionsBuilder<TTenant, TTItem>> configure)
        //where TTenant : class
        //        {
        //            var namedBuilder = new NamedTenantShellItemOptionsBuilder<TTenant, TTItem>(builder);
        //            configure(namedBuilder);
        //            namedBuilder.Build();
        //            return builder;
        //        }


        //        public static IServiceCollection ConfigureNamedTenantShellItems<TTenant, TTItem>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Action<NamedTenantShellItemOptionsBuilder<TTenant, TTItem>> configure)
        //where TTenant : class
        //        {
        //            return optionsBuilder.ConfigureNamedTenantShellItems(configure);
        //        }
    }
}
