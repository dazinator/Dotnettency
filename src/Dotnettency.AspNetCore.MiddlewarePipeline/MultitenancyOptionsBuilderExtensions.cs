using Dotnettency.AspNetCore.MiddlewarePipeline;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class MultitenancyOptionsBuilderExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> ConfigureTenantMiddlewarePipeline<TTenant>(
            this MultitenancyOptionsBuilder<TTenant> builder,
            Action<TenantPipelineOptionsBuilder<TTenant>> configureOptions)
            where TTenant : class
        {

            builder.Services.AddScoped<ITenantPipelineAccessor<TTenant>, TenantPipelineAccessor<TTenant>>();
            var optsBuilder = new TenantPipelineOptionsBuilder<TTenant>(builder);
            configureOptions(optsBuilder);
            return builder;
        }

       
      
    }
}
