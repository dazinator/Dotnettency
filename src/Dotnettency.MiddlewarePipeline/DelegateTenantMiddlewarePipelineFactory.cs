using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;
using Dotnettency.Container;

namespace Dotnettency
{
    public class DelegateTenantMiddlewarePipelineFactory<TTenant> : ITenantMiddlewarePipelineFactory<TTenant>
         where TTenant : class
    {
        // private readonly Func<TTenant, RequestDelegate> _factory;

        private readonly Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> _configuration;

        public DelegateTenantMiddlewarePipelineFactory(Action<TenantPipelineBuilderContext<TTenant>, IApplicationBuilder> configuration)
        {
            _configuration = configuration;
        }

        public async Task<RequestDelegate> Get(IApplicationBuilder appBuilder, TTenant tenant, ITenantContainerAccessor<TTenant> tenantContainerAccessor, RequestDelegate next)
        {
            //return Task.Run(() =>
            //{
            return await BuildTenantPipeline(appBuilder, tenant, tenantContainerAccessor, next);
            //  });
        }

        protected virtual async Task<RequestDelegate> BuildTenantPipeline(IApplicationBuilder rootApp, TTenant tenant, ITenantContainerAccessor<TTenant> tenantContainer, RequestDelegate next)
        {

            var branchBuilder = rootApp.New();
            var appServices = await tenantContainer.TenantContainer.Value;
            branchBuilder.ApplicationServices = appServices.GetServiceProvider();
            var builderContext = new TenantPipelineBuilderContext<TTenant>
            {
                //   TenantContext = tenantContext,
                Tenant = tenant
            };

            _configuration(builderContext, branchBuilder);

            // register root pipeline at the end of the tenant branch
            branchBuilder.Run(next);
            return branchBuilder.Build();
        }
    }




}
