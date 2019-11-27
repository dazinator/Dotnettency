using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Dotnettency.AspNetCore.MiddlewarePipeline
{
    public class DelegateTaskTenantMiddlewarePipelineFactory<TTenant> : ITenantMiddlewarePipelineFactory<TTenant, IApplicationBuilder, RequestDelegate>
    where TTenant : class
    {
        private readonly Func<TenantShellItemBuilderContext<TTenant>, IApplicationBuilder, Task> _configuration;

        public DelegateTaskTenantMiddlewarePipelineFactory(Func<TenantShellItemBuilderContext<TTenant>, IApplicationBuilder, Task> configuration)
        {
            _configuration = configuration;
        }

        public async Task<RequestDelegate> Create(IApplicationBuilder appBuilder, TenantShellItemBuilderContext<TTenant> context, RequestDelegate next, bool reJoin)
        {
            return await UseTenantPipeline(appBuilder, context, next, reJoin);
        }

        protected virtual async Task<RequestDelegate> UseTenantPipeline(IApplicationBuilder rootApp, TenantShellItemBuilderContext<TTenant> context, RequestDelegate next, bool reJoin)
        {
            var branchBuilder = rootApp.New();
            branchBuilder.ApplicationServices = context.Services;           

            await _configuration(context, branchBuilder);

            // register root pipeline at the end of the tenant branch
            if (next != null && reJoin)
            {
                branchBuilder.Run(next);
            }
            return branchBuilder.Build();
        }
    }

}
