using DavidLievrouw.OwinRequestScopeContext;
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
            app.UseRequestScopeContext(); // Always required in middleware pipeline, so that AppBuilderAdaptor service can establish current owin context.
            var adaptor = new AppBuilderAdaptor(app, services);
            var builder = new MultitenancyMiddlewareOptionsBuilder<TTenant>(adaptor);
            configure(builder);
            return app;
        }      

        public static IAppBuilder UseRequestServices(this IAppBuilder builder, IHttpContextProvider httpContextProvider, Func<IServiceScope> factory)
        {
            UseRequestContextItem<IServiceScope>(builder, httpContextProvider, factory, true, (item)=> {
                httpContextProvider.GetCurrent().SetRequestServices(item.ServiceProvider);
            });
            return builder;
        }

        public static IAppBuilder UseRequestContextItem<TItem>(this IAppBuilder builder, IHttpContextProvider httpContextProvider, Func<TItem> factory, bool disposeOfAtEndOfRequest, Action<TItem> onInstanceCreated)
                 where TItem : IDisposable
        {
            var options = new SetRequestContextItemMiddlewareOptions<TItem>()
            {
                HttpContextProvider = httpContextProvider,
                Factory = factory,
                DisposeAtEndOfRequest = disposeOfAtEndOfRequest,
                OnInstanceCreated = onInstanceCreated
            };
            builder.Use(typeof(SetRequestContextItemMiddleware<TItem>), options);
            return builder;
        }

    }
}
