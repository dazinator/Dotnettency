using Dotnettency.AspNetCore.MiddlewarePipeline;
using Dotnettency.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Dotnettency
{
    public static class RouteBuilderExtensions
    {
        public static void MapTenantMiddlewarePipeline<TTenant>(this IRouteBuilder routeBuilder, Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> configuration)
            where TTenant : class
        {
            var logger = routeBuilder.ApplicationBuilder.ApplicationServices.GetRequiredService<ILogger<TenantMiddlewarePipelineRouter<TTenant>>>();
            var tenantShellAccessor = routeBuilder.ApplicationBuilder.ApplicationServices.GetRequiredService<ITenantShellAccessor<TTenant>>();
            var factory = new DelegateTenantMiddlewarePipelineFactory<TTenant>(configuration);
            //var pipelineAccessor = new TenantPipelineAccessor<TTenant>(factory, tenantShellAccessor);        


            var router = new TenantMiddlewarePipelineRouter<TTenant>(nameof(TenantMiddlewarePipelineRouter<TTenant>), logger, routeBuilder.ApplicationBuilder, factory);

            // _builder.Services.AddSingleton<ITenantMiddlewarePipelineFactory<TTenant>>(factory);
           // _builder.Services.AddScoped<ITenantPipelineAccessor<TTenant>, TenantPipelineAccessor<TTenant>>();
         //   return _builder;

            routeBuilder.Routes.Add(router);
        }





    }
}
