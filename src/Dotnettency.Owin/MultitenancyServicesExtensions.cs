using Dotnettency.Middleware;
using Dotnettency.Owin;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public static class MultitenancyServicesExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> AddOwin<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder)
           where TTenant : class
        {
            // this service will only work when 
            //  IAppBuilder.UseRequestScopeContext() middleware has been activated.
            builder.Services.AddSingleton<IHttpContextProvider, HttpContextProvider>();       
            return builder;
        }       
    }
}