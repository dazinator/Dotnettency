using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public abstract class TenantContainerFactory<TTenant> : ITenantContainerFactory<TTenant>
        where TTenant : class
    {
        //  private static readonly ConcurrentDictionary<TenantIdentifier, Task<ITenantContainerAdaptor>> _tenantContainers = new ConcurrentDictionary<TenantIdentifier, Task<ITenantContainerAdaptor>>();

        private readonly IServiceProvider _serviceProvider;

        //  private readonly ConcurrentDictionary<>
        public TenantContainerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ITenantContainerAdaptor> Get(TTenant currentTenant)
        {
            ITenantContainerAdaptor newContainer = await BuildContainer(currentTenant);
            return newContainer;
        }

        protected abstract Task<ITenantContainerAdaptor> BuildContainer(TTenant currentTenant);


    }

}
