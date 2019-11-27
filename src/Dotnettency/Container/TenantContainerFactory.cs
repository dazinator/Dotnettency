using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public abstract class TenantContainerFactory<TTenant> : ITenantContainerFactory<TTenant>
        where TTenant : class
    {

        public TenantContainerFactory()
        {
        }

        public async Task<ITenantContainerAdaptor> Get(TenantShellItemBuilderContext<TTenant> currentTenant)
        {
            return await BuildContainer(currentTenant);
        }

        protected abstract Task<ITenantContainerAdaptor> BuildContainer(TenantShellItemBuilderContext<TTenant> currentTenant);
    }
}
