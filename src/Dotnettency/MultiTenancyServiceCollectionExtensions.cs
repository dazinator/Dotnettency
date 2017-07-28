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
            if (configure != null)
            {
                configure(optionsBuilder);
            }
            //   var serviceProvider = optionsBuilder.Build();

            Func<IServiceProvider> serviceProviderBuilder = optionsBuilder.BuildServiceProvider;
            if (serviceProviderBuilder != null)
            {
                IServiceProvider sp = serviceProviderBuilder();
                return sp;
            }

            return null;
            //return optionsBuilder.ServiceProvider;
        }
    }
}
