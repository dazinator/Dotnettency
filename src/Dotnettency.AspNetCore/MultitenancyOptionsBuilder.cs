using Dotnettency.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public static class MultitenancyOptionsBuilderExtensions       
    {

        public static MultitenancyOptionsBuilder<TTenant> AddDefaultHttpServices<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder)
             where TTenant : class
        {
            builder.Services.AddSingleton<ITenantDistinguisherFactory<TTenant>, RequestAuthorityTenantDistinguisherFactory<TTenant>>();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return builder;
        }     
    }
}
