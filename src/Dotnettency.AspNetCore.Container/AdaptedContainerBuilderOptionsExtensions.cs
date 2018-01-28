using Dotnettency.AspNetCore.Container;
using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public static class AdaptedContainerBuilderOptionsExtensions
    {
        public static AdaptedContainerBuilderOptions<TTenant> AddPerRequestContainerMiddlewareServices<TTenant>(this AdaptedContainerBuilderOptions<TTenant> options)
            where TTenant : class
        {
            options.ContainerBuilderOptions.Builder.Services.AddScoped<ITenantRequestContainerAccessor<TTenant>, TenantRequestContainerAccessor<TTenant>>();
            return options;             
        }      
    }
}
