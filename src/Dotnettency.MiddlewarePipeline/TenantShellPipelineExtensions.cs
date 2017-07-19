using System;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.MiddlewarePipeline
{
    public static class TenantShellPipelineExtensions
    {
        public static Lazy<RequestDelegate> GetOrAddMiddlewarePipeline<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<RequestDelegate> requestDelegateFactory)
            where TTenant : class
        {
            var result = tenantShell.Properties.GetOrAdd(nameof(TenantShellPipelineExtensions), requestDelegateFactory) as Lazy<RequestDelegate>;
            return result;
        }

        //private static RequestDelegate BuildTenantPipeline<TTenant>(TTenant tenant, IApplicationBuilder rootApp, RequestDelegate next;)
        //{

        //    var branchBuilder = rootApp.New();
        //    var builderContext = new TenantPipelineBuilderContext<TTenant>
        //    {

        //        //   TenantContext = tenantContext,
        //        Tenant = tenant
        //    };

        //    configuration(builderContext, branchBuilder);

        //    // register root pipeline at the end of the tenant branch
        //    branchBuilder.Run(next);
        //    return branchBuilder.Build();
        //}


    }


}
