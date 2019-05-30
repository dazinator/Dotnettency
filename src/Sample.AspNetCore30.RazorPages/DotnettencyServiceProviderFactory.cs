using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public class DotnettencyServiceProviderFactory<TTenant> :
        IServiceProviderFactory<IServiceCollection>
        where TTenant : class
    {
      //  private MultitenancyOptionsBuilder<TTenant> _builder;
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            //  var defaultSp = services.BuildServiceProvider();
            //  services.AddSingleton(this);
            return services;

            //var builder = services.AddMultiTenancy<TTenant>((builder) =>
            // {
            //     services.AddSingleton(builder);
            //     _builder = builder;
            // });
            //return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            var factory = TenantServiceProviderFactory<TTenant>.Factory;
            return factory();
            // throw new NotImplementedException();
        }

    }
}

