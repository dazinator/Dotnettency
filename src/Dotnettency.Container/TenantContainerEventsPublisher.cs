using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerEventsPublisher<TTenant> : ITenantContainerEventsPublisher<TTenant>
        where TTenant : class
    {       

        public event Action<Task<TTenant>, IServiceProvider> TenantContainerCreated;

        public event Action<Task<TTenant>, IServiceProvider> NestedTenantContainerCreated;       

        private readonly ITenantAccessor<TTenant> _tenantAccessor;

        public TenantContainerEventsPublisher(ITenantAccessor<TTenant> tenantAccessor)
        {
            _tenantAccessor = tenantAccessor;
        }       

        public void PublishNestedTenantContainerCreated(IServiceProvider serviceProvider)
        {
            var tenant = _tenantAccessor.CurrentTenant?.Value;
            NestedTenantContainerCreated?.Invoke(tenant, serviceProvider);
        }

        public void PublishTenantContainerCreated(IServiceProvider serviceProvider)
        {
            var tenant = _tenantAccessor.CurrentTenant?.Value;
            TenantContainerCreated?.Invoke(tenant, serviceProvider);
        }      
    }
}