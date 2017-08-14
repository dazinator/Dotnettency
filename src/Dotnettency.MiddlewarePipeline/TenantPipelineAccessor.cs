using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.MiddlewarePipeline
{
    public class TenantPipelineAccessor<TTenant> : ITenantPipelineAccessor<TTenant>
         where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;
        private readonly ITenantMiddlewarePipelineFactory<TTenant> _tenantPipelineFactory;

        public TenantPipelineAccessor(
            ITenantMiddlewarePipelineFactory<TTenant> tenantPipelineFactory,           
            TenantShellAccessor<TTenant> tenantShellAccessor)
        {
            _tenantShellAccessor = tenantShellAccessor;
            _tenantPipelineFactory = tenantPipelineFactory;
          
            TenantPipeline = new Func<IApplicationBuilder, RequestDelegate, Lazy<Task<RequestDelegate>>>((appBuilder, next) =>
            {
                var lazy = new Lazy<Task<RequestDelegate>>(async () =>
                {

                    var tenantShell = await _tenantShellAccessor.CurrentTenantShell.Value;
                    if (tenantShell != null)
                    {
                        var tenant = tenantShell?.Tenant;
                        var tenantPipeline = tenantShell.GetOrAddMiddlewarePipeline<TTenant>(new Lazy<Task<RequestDelegate>>(() =>
                        {
                            return _tenantPipelineFactory.Create(appBuilder, tenant, next);
                        }));
                        var requestDelegate = await tenantPipeline.Value;
                        return requestDelegate;
                    }//
                    else
                    {
                        return next;
                        //  _logger.LogDebug("Null tenant shell - No Tenant Middleware Pipeline to execute.");
                        // await _next(context);
                    }
                });
                return lazy;
            });
        }
        
        public Func<IApplicationBuilder, RequestDelegate, Lazy<Task<RequestDelegate>>> TenantPipeline { get; }

    }


}
