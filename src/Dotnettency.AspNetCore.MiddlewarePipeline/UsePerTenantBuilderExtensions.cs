using Dotnettency.AspNetCore;
using Dotnettency.AspNetCore.MiddlewarePipeline;
using Dotnettency.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency
{
    public static class UsePerTenantBuilderExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UsePerTenantMiddlewarePipeline<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder, IApplicationBuilder rootAppBuilder)
            where TTenant : class
        {
            var options = new TenantPipelineMiddlewareOptions() { IsTerminal = false, RootApp = rootAppBuilder };
            builder.ApplicationBuilder.UseMiddleware<TenantPipelineMiddleware<TTenant>>(options);
            return builder;
        }
    }   
}
