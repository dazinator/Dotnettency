using Dotnettency.Container;
using System;

namespace Dotnettency
{
    public static class ConfigureBuilderExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantContainers<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Action<ContainerBuilderOptions<TTenant>> configure)
            where TTenant : class
        {
            var containerOptionsBuilder = new ContainerBuilderOptions<TTenant>(optionsBuilder);
            configure?.Invoke(containerOptionsBuilder);
            return optionsBuilder;
        }
    }
}
