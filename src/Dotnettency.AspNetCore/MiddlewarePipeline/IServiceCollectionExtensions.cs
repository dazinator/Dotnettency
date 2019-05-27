using Dotnettency.AspNetCore.MiddlewarePipeline;
using Dotnettency.Container;
using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public static class IServiceCollectionExtensions
    {
        public static AdaptedContainerBuilderOptions<TTenant> AddPerTenantMiddlewarePipelineServices<TTenant>(this AdaptedContainerBuilderOptions<TTenant> options)
         where TTenant : class
        {
            options.ContainerBuilderOptions.Builder.Services.AddScoped<ITenantPipelineAccessor<TTenant, IApplicationBuilder, RequestDelegate>, TenantPipelineAccessor<TTenant>>();
            return options;
        }
    }
}