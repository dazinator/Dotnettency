using Dotnettency.AspNetCore.Modules.Nancy;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddNancy<TTenant>(this IServiceCollection services)
        where TTenant : class
        {
            services.AddScoped<ITenantNancyBootstrapperFactory<TTenant>, TenantNancyBootstrapperFactory<TTenant>>();
            services.AddScoped<ITenantNancyBootstrapperAccessor<TTenant>, TenantNancyBootstrapperAccessor<TTenant>>();
            return services;
        }

       
    }
}
