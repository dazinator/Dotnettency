using Dotnettency.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dotnettency
{
    public static class RouteBuilderExtensions
    {
        public static void MapTenantMiddlewarePipeline<TTenant>(this IRouteBuilder routeBuilder)
            where TTenant : class
        {
            var logger = routeBuilder.ApplicationBuilder.ApplicationServices.GetRequiredService<ILogger<TenantMiddlewarePipelineRouter<TTenant>>>();
            routeBuilder.Routes.Add(new TenantMiddlewarePipelineRouter<TTenant>(nameof(TenantMiddlewarePipelineRouter<TTenant>), logger, routeBuilder.ApplicationBuilder));
        }
    }
}
