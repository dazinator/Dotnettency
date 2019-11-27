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
        private readonly IServiceProvider _currentScopeServiceProvider;
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;

        public TenantPipelineAccessor(
            IServiceProvider currentScopeServiceProvider,
            ITenantShellAccessor<TTenant> tenantShellAccessor)
        {
            _currentScopeServiceProvider = currentScopeServiceProvider;
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
                        TenantShellItemBuilderContext<TTenant> context = new TenantShellItemBuilderContext<TTenant>()
                        {
                            Services = _currentScopeServiceProvider,
                            Tenant = tenant
                        };
                        return factory.Create(appBuilder, context, next, reJoin);
                    }));

                    return await tenantPipeline.Value;
                });
            });
        }

        public Func<IAppBuilder, IServiceProvider, AppFunc, ITenantMiddlewarePipelineFactory<TTenant, IAppBuilder, AppFunc>, bool, Lazy<Task<AppFunc>>> TenantPipeline { get; private set; }
    }
}
