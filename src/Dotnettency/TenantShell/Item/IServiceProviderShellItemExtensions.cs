using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class IServiceProviderShellItemExtensions
    {
        public static async Task<TItem> GetShellItemAsync<TTenant, TItem>(this IServiceProvider serviceProvider)
            where TTenant : class
        {
            var accessor = serviceProvider.GetRequiredService<ITenantShellItemAccessor<TTenant, TItem>>();
            var accessDelegate = accessor.Factory(serviceProvider);
            var item = await accessDelegate.Value;
            return item;
        }
    }

}
