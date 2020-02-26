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

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var factory = TenantServiceProviderFactory<TTenant>.Factory;
            if (factory == null)
            {
                return containerBuilder.BuildServiceProvider();
            }
            return factory();
        }

    }
}

