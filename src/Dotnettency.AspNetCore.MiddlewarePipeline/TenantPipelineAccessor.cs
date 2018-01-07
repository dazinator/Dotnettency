using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.AspNetCore.MiddlewarePipeline
{
    public class TenantPipelineAccessor<TTenant> : ITenantPipelineAccessor<TTenant>
        where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;

        public TenantPipelineAccessor(            
            ITenantShellAccessor<TTenant> tenantShellAccessor)
        {
            _tenantShellAccessor = tenantShellAccessor;

            TenantPipeline = new Func<IApplicationBuilder, IServiceProvider, RequestDelegate, ITenantMiddlewarePipelineFactory<TTenant>, Lazy<Task<RequestDelegate>>>((appBuilder, sp, next, factory) =>
            {
                return new Lazy<Task<RequestDelegate>>(async () =>
                {
                    var tenantShell = await _tenantShellAccessor.CurrentTenantShell.Value;
                    if (tenantShell == null)
                    {
                        return next;
                    }

                    var tenant = tenantShell?.Tenant;
                    var tenantPipeline = tenantShell.GetOrAddMiddlewarePipeline(new Lazy<Task<RequestDelegate>>(() =>
                    {                       
                        return factory.Create(appBuilder, sp, tenant, next);
                    }));

                    return await tenantPipeline.Value;
                });
            });
        }

        public Func<IApplicationBuilder, IServiceProvider, RequestDelegate, ITenantMiddlewarePipelineFactory<TTenant>, Lazy<Task<RequestDelegate>>> TenantPipeline { get; private set; }
    }
}
