using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantShellItemExtensions
    {
        /// <summary>
        /// Gets or adds an item that is capable of being lazily created for the tenant in an async fashion.
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="tenantShell"></param>
        /// <param name="createLazyAsyncFactory"></param>
        /// <param name="name">Optional name to give to this instance of the item. If not specified, can only store one of this item type.</param>
        /// <returns></returns>
        public static Lazy<Task<TItem>> GetOrAddItem<TTenant, TItem>(this TenantShell<TTenant> tenantShell, Lazy<Task<TItem>> lazyAsyncFactory, string name = "")
            where TTenant : class
        {
            string key = GetKey<TItem>(name);
            return tenantShell.GetOrAddProperty<Lazy<Task<TItem>>>(key, lazyAsyncFactory);
        }

        /// <summary>
        /// Gets or adds an item that is capable of being lazily created for the tenant in an async fashion.
        /// </summary>
        /// <typeparam name="TTenant"></typeparam>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="tenantShell"></param>
        /// <param name="createLazyAsyncFactory"></param>
        /// <param name="name">Optional name to give to this instance of the item. If not specified, can only store one of this item type.</param>

        /// <returns></returns>
        public static Lazy<Task<TItem>> GetOrAddItem<TTenant, TItem>(this TenantShell<TTenant> tenantShell, Func<Lazy<Task<TItem>>> createLazyAsyncFactory, string name = "")
           where TTenant : class
        {
            string key = GetKey<TItem>(name);
            return tenantShell.GetOrAddProperty<Lazy<Task<TItem>>>(key, (s) => createLazyAsyncFactory());
        }

        public static bool TryGetItem<TTenant, TItem>(this TenantShell<TTenant> tenantShell, out Lazy<Task<TItem>> item)
            where TTenant : class
        {
            string key = GetKey<TItem>();
            return tenantShell.TryGetProperty<Lazy<Task<TItem>>>(key, out item);
        }

        private static string GetKey<TItem>(string name = "")
        {
            var key = $"{nameof(TenantShellItemExtensions)}-{typeof(TItem).Name}:{name}";
            return key;
        }

    }
}
