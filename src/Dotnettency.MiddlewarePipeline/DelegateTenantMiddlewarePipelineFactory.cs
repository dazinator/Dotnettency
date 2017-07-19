using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;

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

        public RequestDelegate Get(IApplicationBuilder appBuilder, TTenant tenant, RequestDelegate next)
        {
            //return Task.Run(() =>
            //{
            return BuildTenantPipeline(appBuilder, tenant, next);
            //  });
        }

        protected virtual RequestDelegate BuildTenantPipeline(IApplicationBuilder rootApp, TTenant tenant, RequestDelegate next)
        {

            var branchBuilder = rootApp.New();
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
