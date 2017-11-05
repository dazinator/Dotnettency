using Dotnettency.Container;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantShellContainerExtensions
    {
        public static Lazy<Task<ITenantContainerAdaptor>> GetOrAddContainer<TTenant>(this TenantShell<TTenant> tenantShell, Func<Task<ITenantContainerAdaptor>> containerAdaptorFactory)
            where TTenant : class
        {
            return tenantShell.Properties.GetOrAdd(nameof(TenantShellContainerExtensions), (a) =>
            {
                return new Lazy<Task<ITenantContainerAdaptor>>(containerAdaptorFactory);
            }) as Lazy<Task<ITenantContainerAdaptor>>;
        }
    }
}
