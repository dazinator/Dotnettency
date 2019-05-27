using Dotnettency.Middleware;
using Dotnettency.Owin.MiddlewarePipeline;
using Microsoft.Extensions.DependencyInjection;
using Owin;
using System;

namespace Dotnettency
{
    public static class UsePerTenantBuilderExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UsePerTenantMiddlewarePipeline<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder, IAppBuilder rootAppBuilder, IServiceProvider appServices = null)
            where TTenant : class
        {
            var httpContextProvider = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<IHttpContextProvider>();
            var options = new TenantPipelineMiddlewareOptions() { IsTerminal = false, RootApp = rootAppBuilder, ApplicationServices = appServices, HttpContextProvider = httpContextProvider };
            builder.ApplicationBuilder.UseMiddleware<TenantPipelineMiddleware<TTenant>>(options);
            return builder;
        }
    }
}
