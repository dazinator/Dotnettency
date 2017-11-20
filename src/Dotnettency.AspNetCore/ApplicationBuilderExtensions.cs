using Dotnettency.AspNetCore;
using Microsoft.AspNetCore.Builder;
using System;

namespace Dotnettency
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMultitenancy<TTenant>(this IApplicationBuilder app, Action<MultitenancyMiddlewareOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var builder = new MultitenancyMiddlewareOptionsBuilder<TTenant>(app);
            configure(builder);
            return app;
        }
    }
}