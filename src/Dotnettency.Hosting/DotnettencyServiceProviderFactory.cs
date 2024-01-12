using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public class DotnettencyServiceProviderFactory<TTenant> : IServiceProviderFactory<IServiceCollection>
     where TTenant : class
    {
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            var factory = TenantServiceProviderFactory<TTenant>.Factory;
            if (factory == null)
            {
                return serviceCollection.BuildServiceProvider();
            }
            return factory(serviceCollection);
        }

    }
}

