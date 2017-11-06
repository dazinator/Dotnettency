using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class DelegateTenantMiddlewarePipelineFactory<TTenant> : ITenantMiddlewarePipelineFactory<TTenant>
        where TTenant : class
    {
        private readonly Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> _configuration;

        public DelegateTenantMiddlewarePipelineFactory(Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> configuration)
        {
            _configuration = configuration;
        }

        public async Task<RequestDelegate> Create(IApplicationBuilder appBuilder, TTenant tenant, RequestDelegate next)
        {
            return await BuildTenantPipeline(appBuilder, tenant, next);
        }

        protected virtual Task<RequestDelegate> BuildTenantPipeline(IApplicationBuilder rootApp, TTenant tenant, RequestDelegate next)
        {
            return Task.Run(() =>
            {
                var branchBuilder = rootApp.New();
                var builderContext = new TenantPipelineBuilderContext<TTenant>
                {
                    Tenant = tenant
                };

                _configuration(builderContext, branchBuilder);

                // register root pipeline at the end of the tenant branch
                branchBuilder.Run(next);
                return branchBuilder.Build();
            });
        }
    }
}
