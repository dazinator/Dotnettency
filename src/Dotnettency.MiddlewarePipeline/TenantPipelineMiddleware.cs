using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.MiddlewarePipeline
{

    public class TenantPipelineMiddleware<TTenant>
        where TTenant : class
    {

        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly ITenantMiddlewarePipelineFactory<TTenant> _factory;
       

        public TenantPipelineMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            ITenantMiddlewarePipelineFactory<TTenant> factory)

        {
            _next = next;
            _rootApp = rootApp;
            _factory = factory;
        }


        public async Task Invoke(HttpContext context, ITenantShellAccessor<TTenant> tenantShellAccessor)
        {
            var tenantShell = await tenantShellAccessor.CurrentTenantShell.Value;
            if (tenantShell != null)
            {
                var tenant = tenantShell?.Tenant;
                var tenantPipeline = tenantShell.GetOrAddMiddlewarePipeline<TTenant>(new Lazy<RequestDelegate>(() => {
                    return _factory.Get(_rootApp, tenant, _next);
                }));
                await tenantPipeline.Value(context);
            };
        }        
    }
}
