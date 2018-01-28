using Dotnettency.AspNetCore.MiddlewarePipeline;
using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public static class IServiceCollectionExtensions
    {    

        public static AdaptedContainerBuilderOptions<TTenant> AddPerTenantMiddlewarePipelineServices<TTenant>(this AdaptedContainerBuilderOptions<TTenant> options)
         where TTenant : class
        {
            options.ContainerBuilderOptions.Builder.Services.AddScoped<ITenantPipelineAccessor<TTenant>, TenantPipelineAccessor<TTenant>>();
            return options;
        }
    }

   
}
