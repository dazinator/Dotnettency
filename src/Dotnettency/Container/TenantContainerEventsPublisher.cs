using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerEventsPublisher<TTenant> : ITenantContainerEventsPublisher<TTenant>
        where TTenant : class
    {

        public event Action<Task<TTenant>, IServiceProvider> TenantContainerCreated = null;

        public event Action<Task<TTenant>, IServiceProvider> NestedTenantContainerCreated = null;

        //  private readonly ITenantAccessor<TTenant> _tenantAccessor;

        public TenantContainerEventsPublisher()
        {
            // _tenantAccessor = tenantAccessor;
        }

        public void PublishNestedTenantContainerCreated(IServiceProvider serviceProvider)
        {
            //  var tenant = _tenantAccessor.CurrentTenant?.Value;
            if (NestedTenantContainerCreated != null)
            {
                var accessor = serviceProvider.GetRequiredService<Task<TTenant>>();
                NestedTenantContainerCreated?.Invoke(accessor, serviceProvider);
            }

        }

        public void PublishTenantContainerCreated(IServiceProvider serviceProvider)
        {
            if (TenantContainerCreated != null)
            {
                //tenantShell.CurrentTenantShell.Value.Result.
                 var accessor = serviceProvider.GetRequiredService<Task<TTenant>>();
                TenantContainerCreated?.Invoke(accessor, serviceProvider);
            }
            // var tenant = _tenantAccessor.CurrentTenant?.Value;

        }
    }
}