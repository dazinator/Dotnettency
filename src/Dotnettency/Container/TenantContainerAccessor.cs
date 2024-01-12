using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerAccessor<TTenant> : ITenantContainerAccessor<TTenant>
        where TTenant : class
    {
        private readonly ITenantContainerAdaptor _currentScopeServiceProvider;
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;
        private readonly ITenantContainerFactory<TTenant> _containerFactory;

        public TenantContainerAccessor(ITenantContainerAdaptor currentScopeServiceProvider, ITenantShellAccessor<TTenant> tenantShellAccessor, ITenantContainerFactory<TTenant> factory)
        {
            _currentScopeServiceProvider = currentScopeServiceProvider;
            _tenantShellAccessor = tenantShellAccessor;
            _containerFactory = factory;

            TenantContainer = new Lazy<Task<ITenantContainerAdaptor>>(async () =>
            {
                var tenantShell = await tenantShellAccessor.CurrentTenantShell.Value;

                if (tenantShell == null)
                {
                    return null;
                }

                var tenant = tenantShell?.Tenant;
               
                
                var lazy = tenantShell.GetOrAddContainer(() =>
                {
                    var services = _currentScopeServiceProvider;
                    var context = new TenantShellItemBuilderContext<TTenant>()
                    {
                        Tenant = tenant,
                        Services = services
                    };

                    return factory.Get(context);
                });

                return await lazy.Value;
            });
        }

        public Lazy<Task<ITenantContainerAdaptor>> TenantContainer { get; private set; }
    }
}
