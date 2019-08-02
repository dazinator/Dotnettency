using Dotnettency.TenantFileSystem;
using System;

namespace Dotnettency
{
    public static class MultitenancyServicesExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantFileProviders<TTenant>(
            this MultitenancyOptionsBuilder<TTenant> optionsBuilder,
            Action<TenantFileProviderOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var builder = new TenantFileProviderOptionsBuilder<TTenant>(optionsBuilder);
            configure?.Invoke(builder);
            return optionsBuilder;
        }
    }
}
