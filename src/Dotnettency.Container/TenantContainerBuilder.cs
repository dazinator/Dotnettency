using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
    {
        private readonly ITenantContainerAdaptor _parentContainer;
        private readonly Action<TTenant, IServiceCollection> _configureTenant;

        public TenantContainerBuilder(ITenantContainerAdaptor parentContainer, Action<TTenant, IServiceCollection> configureTenant)
        {
            _parentContainer = parentContainer;
            _configureTenant = configureTenant;
        }

        public Task<ITenantContainerAdaptor> BuildAsync(TTenant tenant)
        {
            var tenantContainer = _parentContainer.CreateChildContainer();

            tenantContainer.Configure(config =>
            {
                _configureTenant(tenant, config);
            });

            return Task.FromResult(tenantContainer);
        }
    }
}
