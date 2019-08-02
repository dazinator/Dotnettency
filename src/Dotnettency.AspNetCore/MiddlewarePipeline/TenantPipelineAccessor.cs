using System;
using System.Threading.Tasks;
using Dotnettency.MiddlewarePipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.AspNetCore.MiddlewarePipeline
{
    public class TenantPipelineAccessor<TTenant> : ITenantPipelineAccessor<TTenant, IApplicationBuilder, RequestDelegate>
        where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;

        public TenantPipelineAccessor(
            ITenantShellAccessor<TTenant> tenantShellAccessor)
        {
            _tenantShellAccessor = tenantShellAccessor;

            TenantPipeline = new Func<IApplicationBuilder, IServiceProvider, RequestDelegate, ITenantMiddlewarePipelineFactory<TTenant, IApplicationBuilder, RequestDelegate>, bool, Lazy<Task<RequestDelegate>>>((appBuilder, sp, next, factory, reJoin) =>
            new Lazy<Task<RequestDelegate>>(async () =>
            {
                var tenantShell = await _tenantShellAccessor.CurrentTenantShell.Value;
                if (tenantShell == null)
                {
                    return next;
                }

                var tenant = tenantShell?.Tenant;

                var tenantPipeline = tenantShell.GetOrAddMiddlewarePipeline(() =>
                    new Lazy<Task<RequestDelegate>>(() =>
                        factory.Create(appBuilder, sp, tenant, next, reJoin)));

                return await tenantPipeline.Value;
            }));
        }


        public Func<IApplicationBuilder, IServiceProvider, RequestDelegate, ITenantMiddlewarePipelineFactory<TTenant, IApplicationBuilder, RequestDelegate>, bool, Lazy<Task<RequestDelegate>>> TenantPipeline { get; private set; }
    }
}
