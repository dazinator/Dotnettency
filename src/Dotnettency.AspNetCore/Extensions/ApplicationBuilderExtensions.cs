using Dotnettency.AspNetCore;
using Dotnettency.Middleware;
using Microsoft.AspNetCore.Builder;
using System;

namespace Dotnettency
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMultitenancy<TTenant>(this IApplicationBuilder app, Action<MultitenancyMiddlewareOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var adaptor = new ApplicationBuilderAdaptor(app);
            var builder = new MultitenancyMiddlewareOptionsBuilder<TTenant>(adaptor);
            configure(builder);
            return app;           
        }
    }
}