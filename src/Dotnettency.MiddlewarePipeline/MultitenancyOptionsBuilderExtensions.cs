using Dotnettency.MiddlewarePipeline;
using System;

namespace Dotnettency
{
    public static class MultitenancyOptionsBuilderExtensions
    {
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
