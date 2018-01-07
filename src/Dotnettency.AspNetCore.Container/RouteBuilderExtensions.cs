using Dotnettency.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Dotnettency
{
    public static class RouteBuilderExtensions
    {
        public static void EnsureTenantContainer<TTenant>(this IRouteBuilder routeBuilder, Action<IRouteBuilder> configureChildRoutes)
            where TTenant : class
        {
            var logger = routeBuilder.ApplicationBuilder.ApplicationServices.GetRequiredService<ILogger<TenantContainerRouter<TTenant>>>();
            routeBuilder.Routes.Add(new TenantContainerRouter<TTenant>(nameof(TenantContainerRouter<TTenant>), logger, routeBuilder.ApplicationBuilder, configureChildRoutes));
        }
    }  
}
