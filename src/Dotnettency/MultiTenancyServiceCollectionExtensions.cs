using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{

    public static class MultitenancyServiceCollectionExtensions
    {
        public static Func<IServiceCollection, IServiceProvider> ServiceProviderFactory;
        public static IServiceCollection AddMultiTenancy<TTenant>(
            this IServiceCollection serviceCollection,
            Action<MultitenancyOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var optionsBuilder = new MultitenancyOptionsBuilder<TTenant>(serviceCollection);
            configure?.Invoke(optionsBuilder);

            ServiceProviderFactory = optionsBuilder.ServiceProviderFactory;

            // Cheat and feels like hack but best I can do at this time.
            TenantServiceProviderFactory<TTenant>.Factory = ServiceProviderFactory;
            return serviceCollection;

            // return (optionsBuilder.ServiceProviderFactory != null)
            //     ? optionsBuilder.ServiceProviderFactory()
            //     : null;
        }
    }
}
