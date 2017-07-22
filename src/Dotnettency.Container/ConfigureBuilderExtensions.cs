using System;
using Dotnettency.Container;

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
            return optionsBuilder;
        }
    }
}
