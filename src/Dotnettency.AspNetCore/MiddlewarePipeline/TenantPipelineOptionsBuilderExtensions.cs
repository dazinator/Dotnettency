using Dotnettency.AspNetCore.MiddlewarePipeline;
using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantPipelineOptionsBuilderExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> AspNetCorePipeline<TTenant>(this TenantPipelineOptionsBuilder<TTenant> builder, Action<TenantShellItemBuilderContext<TTenant>, IApplicationBuilder> configuration)
            where TTenant : class
        {
            var factory = new DelegateTenantMiddlewarePipelineFactory<TTenant>(configuration);
            // builder.
            builder.MultitenancyOptions.Services.AddSingleton<ITenantMiddlewarePipelineFactory<TTenant, IApplicationBuilder, RequestDelegate>>(factory);
            builder.MultitenancyOptions.Services.AddScoped<ITenantPipelineAccessor<TTenant, IApplicationBuilder, RequestDelegate>, TenantPipelineAccessor<TTenant>>();
            return builder.MultitenancyOptions;
        }

        public static MultitenancyOptionsBuilder<TTenant> AspNetCorePipelineTask<TTenant>(this TenantPipelineOptionsBuilder<TTenant> builder, Func<TenantShellItemBuilderContext<TTenant>, IApplicationBuilder, Task> configuration)
          where TTenant : class
        {
            var factory = new DelegateTaskTenantMiddlewarePipelineFactory<TTenant>(configuration);
            // builder.
            builder.MultitenancyOptions.Services.AddSingleton<ITenantMiddlewarePipelineFactory<TTenant, IApplicationBuilder, RequestDelegate>>(factory);
            builder.MultitenancyOptions.Services.AddScoped<ITenantPipelineAccessor<TTenant, IApplicationBuilder, RequestDelegate>, TenantPipelineAccessor<TTenant>>();
            return builder.MultitenancyOptions;
        }
    }
}
