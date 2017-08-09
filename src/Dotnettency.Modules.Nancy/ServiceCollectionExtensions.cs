using Microsoft.Extensions.DependencyInjection;
using Sample;

namespace Dotnettency.Modules.Nancy
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
