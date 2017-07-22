using System;
using System.Threading.Tasks;
using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public static class ConfigureBuilderExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantContainers<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Action<ContainerBuilderOptions<TTenant>> configure)
            where TTenant : class
        {
            var containerOptionsBuilder = new ContainerBuilderOptions<TTenant>(optionsBuilder);
            if (configure != null)
            {
                configure(containerOptionsBuilder);
            }

            //  containerOptionsBuilder.Builder.ServiceProvider = containerOptionsBuilder.Buil

            //   var serviceProvider = optionsBuilder.Build();
            return optionsBuilder;
        }
    }
}
