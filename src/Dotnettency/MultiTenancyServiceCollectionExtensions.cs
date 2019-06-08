using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{

    public static class MultitenancyServiceCollectionExtensions
    {
        public static Func<IServiceProvider> ServiceProviderFactory;
        public static IServiceProvider AddMultiTenancy<TTenant>(
            this IServiceCollection serviceCollection,
            Action<MultitenancyOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var optionsBuilder = new MultitenancyOptionsBuilder<TTenant>(serviceCollection);
            configure?.Invoke(optionsBuilder);

            ServiceProviderFactory = optionsBuilder.ServiceProviderFactory;

            // Cheat and feels like hack but best I can do at this time.
            TenantServiceProviderFactory<TTenant>.Factory = ServiceProviderFactory;           

            return (optionsBuilder.ServiceProviderFactory != null)
                ? optionsBuilder.ServiceProviderFactory()
                : null;
        }
    }
}
