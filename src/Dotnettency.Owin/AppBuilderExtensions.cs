using Dotnettency.Middleware;
using Dotnettency.Owin;
using Microsoft.Extensions.DependencyInjection;
using Owin;
using System;

namespace Dotnettency
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseMultitenancy<TTenant>(this IAppBuilder app, IServiceProvider services, Action<MultitenancyMiddlewareOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var adaptor = new AppBuilderAdaptor(app, services);
            var builder = new MultitenancyMiddlewareOptionsBuilder<TTenant>(adaptor);
            configure(builder);
            return app;
        }
               
    }
}
