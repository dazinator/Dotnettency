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
                        return _tenantPipelineFactory.Create(appBuilder, tenant, next);
                    }));

                    return await tenantPipeline.Value;
                });
            });
        }

        public Func<IApplicationBuilder, RequestDelegate, Lazy<Task<RequestDelegate>>> TenantPipeline { get; private set; }
    }
}
