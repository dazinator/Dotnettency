using Dotnettency.HostingEnvironment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class ConfigurePerHostingEnvironmentBuilderExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> ConfigurePerTenantHostingEnvironment<TTenant>(this MultitenancyOptionsBuilder<TTenant> optionsBuilder, IHostingEnvironment env, Action<TenantHostingEnvironmentOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var builder = new TenantHostingEnvironmentOptionsBuilder<TTenant>(optionsBuilder, env);
            configure?.Invoke(builder);
            return optionsBuilder;
        }

        //public static IServiceCollection ConfigurePerTenantHostingEnvironment<TTenant>(this IServiceCollection services, IHostingEnvironment env, Action<TenantHostingEnvironmentOptionsBuilder<TTenant>> configure)
        //   where TTenant : class
        //{

        //    services.AddScoped<IHostingEnvironment>((sp) =>
        //    {
        //        return new TenantHostingEnvironment<TTenant>(env);
        //    });

        //    var builder = new TenantHostingEnvironmentOptionsBuilder<TTenant>(services, env);
        //    configure?.Invoke(builder);
        //    return services;
        //}
    }
}
