using Dotnettency.Middleware;
using Dotnettency.Owin;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class MultitenancyMiddlewareExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UseRequestServices<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder, Func<IServiceScope> factory)
        {
            UseRequestContextItem<TTenant, IServiceScope>(builder, factory, true);
            return builder;
        }

        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UseRequestContextItem<TTenant, TItem>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder, Func<TItem> factory, bool disposeOfAtEndOfRequest)
         where TItem : IDisposable
        {
            var httpContextProvider = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<IHttpContextProvider>();
            var options = new SetRequestContextItemMiddlewareOptions<TItem>()
            {
                HttpContextProvider = httpContextProvider,
                Factory = factory,
                DisposeAtEndOfRequest = disposeOfAtEndOfRequest
            };

            builder.ApplicationBuilder.UseMiddleware<SetRequestContextItemMiddleware<TItem>>(options);
            return builder;
        }

    }
}
