using Dotnettency.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceProvider AddAspNetCoreMultiTenancy<TTenant>(this IServiceCollection serviceCollection, Action<MultitenancyOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {

            var sp = serviceCollection.AddMultiTenancy<TTenant>((builder) =>
            {
                builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                builder.Services.AddSingleton<ITenantDistinguisherFactory<TTenant>, RequestAuthorityTenantDistinguisherFactory<TTenant>>();

                configure(builder);
            });

            return sp;
        }
    }

}