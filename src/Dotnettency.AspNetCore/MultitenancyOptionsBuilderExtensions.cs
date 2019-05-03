using Dotnettency.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public static class MultitenancyOptionsBuilderExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> AddAspNetCore<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder)
           where TTenant : class
        {
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddSingleton<IHttpContextProvider, HttpContextProvider>();            
            return builder;
        }               
    }
}