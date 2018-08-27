using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
        where TTenant : class
    {
        private readonly ITenantContainerAdaptor _parentContainer;
        private readonly Action<TTenant, IServiceCollection> _configureTenant;
        private readonly ITenantContainerEventsPublisher<TTenant> _containerEventsPublisher;      

        public TenantContainerBuilder(ITenantContainerAdaptor parentContainer,
            Action<TTenant, IServiceCollection> configureTenant,
            ITenantContainerEventsPublisher<TTenant> containerEventsPublisher)
        {
            _parentContainer = parentContainer;
            _configureTenant = configureTenant;
            _containerEventsPublisher = containerEventsPublisher;           
        }

        public Task<ITenantContainerAdaptor> BuildAsync(TTenant tenant)
        {
            var tenantContainer = _parentContainer.CreateChildContainerAndConfigure("Tenant: " + tenant.ToString(), config =>
            {
                _configureTenant(tenant, config);
            });

           // tenantContainer.Configure();

            _containerEventsPublisher?.PublishTenantContainerCreated(tenantContainer);

            return Task.FromResult(tenantContainer);
        }
    }
}
