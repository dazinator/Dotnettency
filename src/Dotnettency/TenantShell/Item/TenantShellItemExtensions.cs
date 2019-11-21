using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantShellItemExtensions
    {
        public static Lazy<Task<TItem>> GetOrAddItem<TTenant, TItem>(this TenantShell<TTenant> tenantShell, Lazy<Task<TItem>> lazyAsyncFactory)
            where TTenant : class
        {
            return tenantShell.GetOrAddProperty<Lazy<Task<TItem>>>(nameof(TenantShellItemExtensions) + typeof(TItem).Name, lazyAsyncFactory);
        }

        public static Lazy<Task<TItem>> GetOrAddItem<TTenant, TItem>(this TenantShell<TTenant> tenantShell, Func<Lazy<Task<TItem>>> createLazyAsyncFactory)
           where TTenant : class
        {
            return tenantShell.GetOrAddProperty<Lazy<Task<TItem>>>(nameof(TenantShellItemExtensions) + typeof(TItem).Name, (s) => createLazyAsyncFactory());
        }
    }



}
