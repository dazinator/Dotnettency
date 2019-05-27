using System;
using System.Threading.Tasks;
using Dotnettency.MiddlewarePipeline;
using Owin;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Dotnettency.Owin.MiddlewarePipeline
{
    public class TenantPipelineAccessor<TTenant> : ITenantPipelineAccessor<TTenant, IAppBuilder, AppFunc>
        where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;

        public TenantPipelineAccessor(
            ITenantShellAccessor<TTenant> tenantShellAccessor)
        {
            _tenantShellAccessor = tenantShellAccessor;

            TenantPipeline = new Func<IAppBuilder, IServiceProvider, AppFunc, ITenantMiddlewarePipelineFactory<TTenant, IAppBuilder, AppFunc>, bool, Lazy<Task<AppFunc>>>((appBuilder, sp, next, factory, reJoin) =>
            {
                return new Lazy<Task<AppFunc>>(async () =>
                {
                    var tenantShell = await _tenantShellAccessor.CurrentTenantShell.Value;
                    if (tenantShell == null)
                    {
                        return next;
                    }

                    var tenant = tenantShell?.Tenant;
                    var tenantPipeline = tenantShell.GetOrAddMiddlewarePipeline(new Lazy<Task<AppFunc>>(() =>
                    {
                        return factory.Create(appBuilder, sp, tenant, next, reJoin);
                    }));

                    return await tenantPipeline.Value;
                });
            });
        }

        public Func<IAppBuilder, IServiceProvider, AppFunc, ITenantMiddlewarePipelineFactory<TTenant, IAppBuilder, AppFunc>, bool, Lazy<Task<AppFunc>>> TenantPipeline { get; private set; }
    }
}
