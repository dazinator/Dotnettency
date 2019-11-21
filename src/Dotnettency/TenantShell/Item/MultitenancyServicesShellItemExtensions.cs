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
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantShellItem<TTenant, TTItem>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Func<TenantShellItemBuilderContext<TTenant>, TTItem> configureItem, string name = "")
             where TTenant : class
        {
            var factory = new DelegateTenantShellItemFactory<TTenant, TTItem>(configureItem);

            // optionsBuilder.Services.AddSingleton<ITenantShellItemFactory<TTenant, TTItem>>(factory);

            optionsBuilder.Services.AddScoped<ITenantShellItemAccessor<TTenant, TTItem>>((sp) =>
            {
                var shellAccessor = sp.GetRequiredService<ITenantShellAccessor<TTenant>>();
                var fact = factory;
                return new TenantShellItemAccessor<TTenant, TTItem>(shellAccessor, fact);
            });

            // allow Task<IConfiguration> to be resolved for getting the current tenants IConfiguration in a non-blocking mannor;

            // Support injection of Task<TItem> - a convenience that allows non blocking access to this shell item for the tenant.
            // when required - minor contention on Lazy<>
            optionsBuilder.Services.AddScoped(sp =>
            {
                return sp.GetRequiredService<ITenantShellItemAccessor<TTenant, TTItem>>().Factory(sp).Value;
            });

            return optionsBuilder;
        }

    }
}
