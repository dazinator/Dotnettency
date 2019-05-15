using Dotnettency.Container;
using Dotnettency.Middleware;
using Dotnettency.Owin;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class MultitenancyMiddlewareExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UseRequestServices<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder, Func<IServiceScope> factory)
            where TTenant : class
        {           
            UseRequestContextItem<TTenant, IServiceScope>(builder, factory, true, (scope)=> {
                // Set the request scoped IServiceProvider as the current .RequestServices for the environment.
                // In ASP.NET Core this is HttpContext.RequestServices, in OWIN, we store it in an Items dictionary.
                var swapper = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<RequestServicesSwapper<TTenant>>();
                swapper.SwapRequestServices(scope.ServiceProvider);
            });
            return builder;
        }

        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UseRequestContextItem<TTenant, TItem>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder, Func<TItem> factory, bool disposeOfAtEndOfRequest, Action<TItem> onInstanceCreated = null)
         where TItem : IDisposable
        {
            var httpContextProvider = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<IHttpContextProvider>();
            var options = new SetRequestContextItemMiddlewareOptions<TItem>()
            {
                HttpContextProvider = httpContextProvider,
                Factory = factory,
                DisposeAtEndOfRequest = disposeOfAtEndOfRequest,
                OnInstanceCreated = onInstanceCreated
            };

            builder.ApplicationBuilder.UseMiddleware<SetRequestContextItemMiddleware<TItem>>(options);
            return builder;
        }


        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UseTenantContainers<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder)
           where TTenant : class
        {
            var options = new TenantContainerMiddlewareOptions()
            {
                DisposeAtEndOfRequest = true,
                HttpContextProvider = builder.ApplicationBuilder.ApplicationServices.GetRequiredService<IHttpContextProvider>()
            };
            builder.ApplicationBuilder.UseMiddleware<TenantContainerMiddleware<TTenant>>(options);
            return builder;
        }
    }
}
