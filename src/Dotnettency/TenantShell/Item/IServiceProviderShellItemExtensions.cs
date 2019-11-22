using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class IServiceProviderShellItemExtensions
    {
        public static async Task<TItem> GetShellItemAsync<TTenant, TItem>(this IServiceProvider serviceProvider, string name = "")
            where TTenant : class
        {
            Lazy<Task<TItem>> lazyFactory = null;
            if (string.IsNullOrEmpty(name))
            {
                var accessor = serviceProvider.GetRequiredService<ITenantShellItemAccessor<TTenant, TItem>>();
                lazyFactory = accessor.Factory(serviceProvider);
            }
            else
            {
                var accessor = serviceProvider.GetRequiredService<ITenantShellNamedItemAccessor<TTenant, TItem>>();
                lazyFactory = accessor.NamedFactory(serviceProvider, name);
            }

            var item = await lazyFactory.Value;
            return item;

        }

    }
}
