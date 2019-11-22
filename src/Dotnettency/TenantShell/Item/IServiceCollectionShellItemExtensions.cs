using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class IServiceCollectionShellItemExtensions
    {
        public static IServiceCollection ConfigureTenantShellItem<TTenant, TTItem>(this IServiceCollection services, Func<TenantShellItemBuilderContext<TTenant>, TTItem> configureItem)
    where TTenant : class
        {
            var factory = new DelegateTenantShellItemFactory<TTenant, TTItem>(configureItem);

            // optionsBuilder.Services.AddSingleton<ITenantShellItemFactory<TTenant, TTItem>>(factory);

            services.AddScoped<ITenantShellItemAccessor<TTenant, TTItem>>((sp) =>
            {
                var shellAccessor = sp.GetRequiredService<ITenantShellAccessor<TTenant>>();
                var fact = factory;
                return new TenantShellItemAccessor<TTenant, TTItem>(shellAccessor, fact);
            });

            // Support injection of Task<TItem> - a convenience that allows non blocking access to this shell item for the tenant.
            // when required - minor contention on Lazy<>
            services.AddScoped(sp =>
            {
                return sp.GetRequiredService<ITenantShellItemAccessor<TTenant, TTItem>>().Factory(sp).Value;
            });

            return services;
        }
        
    }
}
