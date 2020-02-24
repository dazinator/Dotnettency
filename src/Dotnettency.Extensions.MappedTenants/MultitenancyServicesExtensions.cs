using Dotnettency.Extensions.MappedTenants;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{

    public static class MultitenancyServicesExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> IdentifyTenantsUsingRequestAuthorityMapping<TTenant, TTenantMappingOptions, TKey>(this MultitenancyOptionsBuilder<TTenant> builder)
            where TTenant : class
            where TTenantMappingOptions : TenantMappingOptions<TKey>
        {
            builder.Services.AddSingleton<TenantMatcherFactory<TKey>>(); // could be overriden to load mappings in a custom way from the configured options, i.e to use custom pattern mathing logic.
            builder.IdentifyTenantsWith<MappedRequestAuthorityTenantIdentifierFactory<TTenant, TKey>>();
            return builder;
        }
    }
}