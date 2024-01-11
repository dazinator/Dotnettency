using Dotnettency.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Dotnettency.Options;

namespace Dotnettency
{
    public static class MultitenancyServicesExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> AddCoreAspNetCore<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder)
           where TTenant : class
        {
            var httpContextAccesser = builder.Services.FindServiceInstance<IHttpContextAccessor>();
            if (httpContextAccesser == null)
            {
                httpContextAccesser = new HttpContextAccessor();
                builder.Services.AddSingleton<IHttpContextAccessor>(httpContextAccesser);
            }

            var provider = new HttpContextProvider(httpContextAccesser);
            builder.SetHttpContextProvider(provider);
            return builder;
        }
        
        public static MultitenancyOptionsBuilder<TTenant> AddAspNetCore<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder)
            where TTenant : class
        {
            builder.AddCoreAspNetCore<TTenant>();
            builder.SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>));
            return builder;
        }
    }
    
    
}