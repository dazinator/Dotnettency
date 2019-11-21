using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Dotnettency
{
    public static class TenantContainerBuilderContextShellItemExtensions
    {
        public static async Task<TItem> GetShellItemAsync<TTenant, TItem>(this TenantContainerBuilderContext<TTenant> context)
            where TTenant : class
        {
            var sp = context.ApplicationServices;
            var accessor = sp.GetRequiredService<ITenantShellItemAccessor<TTenant, TItem>>();
            var accessDelegate = accessor.Factory(sp);
            var item = await accessDelegate.Value;
            return item;
        }
    }

}
