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

        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UseCurrentRequestItem<TTenant, TItem>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder, Func<TItem> factory)
                 where TItem : IDisposable
        {
            var httpContextProvider = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<IHttpContextProvider>();
            builder.ApplicationBuilder.UseMiddleware<RequestScopeItemMiddleware<TItem>>(httpContextProvider, factory);
            
          //  app.Use(typeof(RequestScopeMiddleware), factory);
            //builder.ApplicationBuilder.UseMiddleware<RequestScopeMiddleware>(scopedContainerFactory);
            return builder;
        }

        public static IServiceCollection AddCurrentRequestItemAccessor<TItem>(this IServiceCollection services, Func<IServiceProvider, TItem> factory = null)
                 where TItem : class, IDisposable
        {

            services.AddSingleton<ICurrentRequestItemAccessor<TItem>>(sp =>
            {
                return new CurrentRequestItemAccessor<TItem>(sp.GetRequiredService<IHttpContextProvider>(), sp, factory);
            });
            return services;
        }




    }

    public class CurrentRequestItemAccessor<TItem> : ICurrentRequestItemAccessor<TItem>
        where TItem : class, IDisposable
    {
        private readonly IHttpContextProvider _httpContextProvider;
        private readonly IServiceProvider _sp;
        private readonly Func<IServiceProvider, TItem> _factory;
        private readonly bool _disposeOnRequestCompletion;

        public CurrentRequestItemAccessor(IHttpContextProvider httpContextProvider, IServiceProvider sp, Func<IServiceProvider, TItem> factory, bool disposeOnRequestCompletion = true)
        {
            _httpContextProvider = httpContextProvider;
            _sp = sp;
            _factory = factory;
            _disposeOnRequestCompletion = disposeOnRequestCompletion;
        }

        public TItem GetCurrent()
        {
            // Gte current http context,
            // if no http context then return null;
            var context = _httpContextProvider.GetCurrent();
            if (context == null)
            {
                return null;
            }

            // get item if it exists,
            var item = context.GetItem<TItem>(nameof(TItem));
            if(item == null)
            {
                // consider locking
                item = _factory?.Invoke(_sp);
                if(item != null)
                {
                    context.SetItem(nameof(TItem), item, _disposeOnRequestCompletion);
                }
            }
            return item;
            //if(item == null)
            //{

            //}
            //// if it doesn't exist, call factory to create it and store it in current request context, to be disposed of at end of request.
            //throw new NotImplementedException();
        }
    }


    public interface ICurrentRequestItemAccessor<TItem>
        where TItem : IDisposable
    {
        TItem GetCurrent();
    }
}
