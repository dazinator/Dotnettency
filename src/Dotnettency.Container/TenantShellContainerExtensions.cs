using System;
using System.Threading.Tasks;
using Dotnettency.Container;

namespace Dotnettency
{
    public static class TenantShellContainerExtensions
    {
        public static Lazy<Task<ITenantContainerAdaptor>> GetOrAddContainer<TTenant>(this TenantShell<TTenant> tenantShell, Func<Task<ITenantContainerAdaptor>> containerAdaptorFactory)
            where TTenant : class
        {
            var result = tenantShell.Properties.GetOrAdd(nameof(TenantShellContainerExtensions),
                (a) =>
                {
                    //  var factory = containerAdaptorFactory();
                    return new Lazy<Task<ITenantContainerAdaptor>>(containerAdaptorFactory);
                }) as Lazy<Task<ITenantContainerAdaptor>>;
            return result;
        }
    }
}