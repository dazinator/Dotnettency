using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Dotnettency.AspNetCore.MiddlewarePipeline
{
    public class DelegateTenantMiddlewarePipelineFactory<TTenant> : ITenantMiddlewarePipelineFactory<TTenant, IApplicationBuilder, RequestDelegate>
        where TTenant : class
    {
        private readonly Action<TenantShellItemBuilderContext<TTenant>, IApplicationBuilder> _configuration;

        public DelegateTenantMiddlewarePipelineFactory(Action<TenantShellItemBuilderContext<TTenant>, IApplicationBuilder> configuration)
        {
            _configuration = configuration;
        }

        public async Task<RequestDelegate> Create(IApplicationBuilder appBuilder, TenantShellItemBuilderContext<TTenant> context, RequestDelegate next, bool reJoin)
        {
            return await UseTenantPipeline(appBuilder, context, next, reJoin);
        }

        protected virtual Task<RequestDelegate> UseTenantPipeline(IApplicationBuilder rootApp, TenantShellItemBuilderContext<TTenant> context, RequestDelegate next, bool reJoin)
        {
            return Task.Run(() =>
            {
                var branchBuilder = rootApp.New();
                branchBuilder.ApplicationServices = context.Services;
               
                _configuration(context, branchBuilder);

                // register root pipeline at the end of the tenant branch
                if (next != null && reJoin)
                {
                    branchBuilder.Run(next);
                }
                return branchBuilder.Build();
            });
        }
    }

}
