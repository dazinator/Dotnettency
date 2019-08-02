using Dotnettency.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class MultitenancyServicesExtensions
    {
        //public static MultitenancyOptionsBuilder<TTenant> ConfigureTenant<TTenant>(
        //    this MultitenancyOptionsBuilder<TTenant> optionsBuilder,
        //    Action<TenantConfigurationOptionsBuilder<TTenant>> configure)
        //    where TTenant : class
        //{
        //    var builder = new TenantConfigurationOptionsBuilder<TTenant>(optionsBuilder);
        //    configure?.Invoke(builder);
        //    return optionsBuilder;
        //}

        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantConfiguration<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, Func<TenantConfigurationBuilderContext<TTenant>, IConfigurationBuilder> configureTenantDelegate)
             where TTenant : class
        {
            var factory = new DelegateTenantConfigurationFactory<TTenant>(configureTenantDelegate);
            optionsBuilder.Services.AddSingleton<ITenantConfigurationFactory<TTenant, IConfiguration>>(factory);
            optionsBuilder.Services.AddScoped<ITenantConfigurationAccessor<TTenant, IConfiguration>, TenantConfigurationAccessor<TTenant>>();

            // allow Task<IConfiguration> to be resolved for getting the current tenants IConfiguration in a non-blocking mannor;

            // Support injection of Task<IConfiguration> - a convenience that allows non blocking access to tenants IConfiguration
            // when required - minor contention on Lazy<>
            optionsBuilder.Services.AddScoped(sp => {
                return sp.GetRequiredService<ITenantConfigurationAccessor<TTenant, IConfiguration>>().ConfigFactory(sp).Value;
            });

            return optionsBuilder;
        }

    }
}
