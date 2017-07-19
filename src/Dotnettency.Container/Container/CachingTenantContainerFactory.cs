using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WebExperiment
{
    public abstract class CachingTenantContainerFactory<TTenant> : ITenantContainerFactory<TTenant>
        where TTenant : class
    {
        private static readonly ConcurrentDictionary<TenantIdentifier, Task<ITenantContainerAdaptor>> _tenantContainers = new ConcurrentDictionary<TenantIdentifier, Task<ITenantContainerAdaptor>>();

       private readonly IServiceProvider _serviceProvider;

        //  private readonly ConcurrentDictionary<>
        public CachingTenantContainerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task<ITenantContainerAdaptor> Get(TenantIdentifier identifier)
        {
            var container = _tenantContainers.GetOrAdd(identifier, async (id) =>
            {
                // build and return a container for this tenant.
                var tenantAccessor = _serviceProvider.GetRequiredService<ITenantAccessor<TTenant>>();
                var currentTenant = await tenantAccessor.Tenant;
                ITenantContainerAdaptor newContainer = await BuildContainer(currentTenant);
                return newContainer;
            });

            return container;

        }

        protected abstract Task<ITenantContainerAdaptor> BuildContainer(TTenant currentTenant);


    }

}
