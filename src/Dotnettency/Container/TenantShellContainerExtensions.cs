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
            return tenantShell.GetOrAddProperty(nameof(TenantShellContainerExtensions), (a) =>
            {
                var newItem = new Lazy<Task<ITenantContainerAdaptor>>(containerAdaptorFactory);
                tenantShell.RegisterCallbackOnDispose(() => {
                    if(newItem.IsValueCreated)
                    {
                        var result = newItem.Value.Result;
                        result?.Dispose();
                    }
                });
                return newItem;
            }) as Lazy<Task<ITenantContainerAdaptor>>;
        }
    }
}
