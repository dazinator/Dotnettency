using System;
using System.Threading.Tasks;
using Dotnettency.Container;

namespace Dotnettency
{
    public static class TenantShellContainerExtensions
    {
        public static Lazy<Task<ITenantContainerAdaptor>> GetOrAddContainer<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<Task<ITenantContainerAdaptor>> containerAdaptor)
            where TTenant : class
        {           
            var result = tenantShell.Properties.GetOrAdd(nameof(TenantShellContainerExtensions), containerAdaptor) as Lazy<Task<ITenantContainerAdaptor>>;
            return result;
        }
    }
}