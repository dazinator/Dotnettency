using Dotnettency.Configuration;
using System;

namespace Dotnettency
{
    public static class MultitenancyServicesExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenant<TTenant>(
            this MultitenancyOptionsBuilder<TTenant> optionsBuilder,
            Action<TenantConfigurationOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var builder = new TenantConfigurationOptionsBuilder<TTenant>(optionsBuilder);
            configure?.Invoke(builder);
            return optionsBuilder;
        }
    }
}
