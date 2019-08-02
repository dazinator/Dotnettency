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
        private readonly Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> _configuration;

        public DelegateTenantMiddlewarePipelineFactory(Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> configuration)
        {
            _configuration = configuration;
        }

        public async Task<RequestDelegate> Create(IApplicationBuilder appBuilder, IServiceProvider serviceProviderOverride, TTenant tenant, RequestDelegate next, bool reJoin)
        {
            return await UseTenantPipeline(appBuilder, serviceProviderOverride, tenant, next, reJoin);
        }

        protected virtual Task<RequestDelegate> UseTenantPipeline(IApplicationBuilder rootApp, IServiceProvider serviceProviderOverride, TTenant tenant, RequestDelegate next, bool reJoin)
        {
            return Task.Run(() =>
            {
                var branchBuilder = rootApp.New();
                branchBuilder.ApplicationServices = serviceProviderOverride;
                var builderContext = new TenantPipelineBuilderContext<TTenant>
                {
                    Tenant = tenant
                };

                _configuration(builderContext, branchBuilder);

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
