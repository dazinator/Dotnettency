using Dotnettency.HostingEnvironment;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Dotnettency
{
    public static class ConfigurePerHostingEnvironmentBuilderExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> ConfigurePerTenantHostingEnvironment<TTenant>(
            this MultitenancyOptionsBuilder<TTenant> optionsBuilder,
            IHostingEnvironment env,
            Action<TenantHostingEnvironmentOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var builder = new TenantHostingEnvironmentOptionsBuilder<TTenant>(optionsBuilder, env);
            configure?.Invoke(builder);
            return optionsBuilder;
        }
    }
}
