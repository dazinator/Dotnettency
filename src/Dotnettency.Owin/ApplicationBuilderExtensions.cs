using Owin;
using System;

namespace Dotnettency.Owin
{
    public static class ApplicationBuilderExtensions
    {
        public static IAppBuilder UseMultitenancy<TTenant>(this IAppBuilder app, Action<MultitenancyMiddlewareOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var builder = new MultitenancyMiddlewareOptionsBuilder<TTenant>(app);
            configure(builder);
            return app;
        }
    }
}
