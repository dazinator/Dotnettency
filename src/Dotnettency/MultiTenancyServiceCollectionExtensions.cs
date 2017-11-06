using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class MultitenancyServiceCollectionExtensions
    {
        public static IServiceProvider AddMultiTenancy<TTenant>(this IServiceCollection serviceCollection, Action<MultitenancyOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var optionsBuilder = new MultitenancyOptionsBuilder<TTenant>(serviceCollection);
            configure?.Invoke(optionsBuilder);
            
            return (optionsBuilder.ServiceProviderFactory != null)
                ? optionsBuilder.ServiceProviderFactory()
                : null;
        }
    }
}
