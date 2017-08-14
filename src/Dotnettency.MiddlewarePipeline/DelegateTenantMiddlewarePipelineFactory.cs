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

        public async Task<RequestDelegate> Create(IApplicationBuilder appBuilder, TTenant tenant, RequestDelegate next)
        {
            //return Task.Run(() =>
            //{
            return await BuildTenantPipeline(appBuilder, tenant, next);
            //  });
        }

        protected virtual Task<RequestDelegate> BuildTenantPipeline(IApplicationBuilder rootApp, TTenant tenant, RequestDelegate next)
        {
            return Task.Run<RequestDelegate>(() =>
             {

                 var branchBuilder = rootApp.New();
                 // var appServices = await tenantContainer.TenantContainer.Value;
                 // branchBuilder.ApplicationServices = appServices;
                 var builderContext = new TenantPipelineBuilderContext<TTenant>
                 {
                     //   TenantContext = tenantContext,
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
