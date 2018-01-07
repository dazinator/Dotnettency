using Dotnettency.AspNetCore.MiddlewarePipeline;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class MultitenancyOptionsBuilderExtensions
    {
        [Obsolete("Use AddPerTenantMiddleware() instead, and then in Configure() call the the new IRouteBuilder extensions methods to route through a tenant middleware pipeline.")]
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantMiddleware<TTenant>(
            this MultitenancyOptionsBuilder<TTenant> builder,
            Action<TenantPipelineOptionsBuilder<TTenant>> configureOptions)
            where TTenant : class
        {
            var optsBuilder = new TenantPipelineOptionsBuilder<TTenant>(builder);
            configureOptions(optsBuilder);
            return builder;
        }

       
      
    }
}
