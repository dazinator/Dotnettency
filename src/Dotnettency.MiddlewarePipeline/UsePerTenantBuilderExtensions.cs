using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency
{
    public static class UsePerTenantBuilderExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UsePerTenantMiddlewarePipeline<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder)
            where TTenant : class
        {
            builder.ApplicationBuilder.UseMiddleware<TenantPipelineMiddleware<TTenant>>(builder.ApplicationBuilder);
            return builder;
        }
    }
}
