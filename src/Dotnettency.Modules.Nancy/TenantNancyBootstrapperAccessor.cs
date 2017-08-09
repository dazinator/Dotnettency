using Dotnettency;
using Dotnettency.Modules.Nancy;
using System;
using System.Threading.Tasks;

namespace Sample
{
    public class TenantNancyBootstrapperAccessor<TTenant> : ITenantNancyBootstrapperAccessor<TTenant>
       where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;
        private readonly ITenantNancyBootstrapperFactory<TTenant> _factory;

        public TenantNancyBootstrapperAccessor(ITenantShellAccessor<TTenant> tenantShellAccessor, ITenantNancyBootstrapperFactory<TTenant> factory)
        {
            _tenantShellAccessor = tenantShellAccessor;
            _factory = factory;

            Bootstrapper = new Lazy<Task<TenantContainerNancyBootstrapper<TTenant>>>(async () =>
            {
                // return new Task<TTenant>(async () =>
                //{
                var tenantShell = await tenantShellAccessor.CurrentTenantShell.Value;
                if (tenantShell == null)
                {
                    return null;
                }

                var tenant = tenantShell?.Tenant;
                var lazy = tenantShell.GetOrAddNancyBootstrapper<TTenant>(() =>
                {
                    return factory.Get(tenant);
                });
                var container = await lazy.Value;
                return container;
            });
        }

        public Lazy<Task<TenantContainerNancyBootstrapper<TTenant>>> Bootstrapper { get; private set; }

    }
}