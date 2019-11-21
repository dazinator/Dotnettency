using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantShellItemBuilderContext<TTenant>
        where TTenant : class
    {
        public TenantShellItemBuilderContext()
        {

        }

        public TTenant Tenant { get; set; }
        
        public IServiceProvider Services { get; set; }

        public async Task<TItem> GetShellItemAsync<TItem>()
        {
            var accessor = Services.GetRequiredService<ITenantShellItemAccessor<TTenant, TItem>>();
            var accessDelegate = accessor.Factory(Services);
            var item = await accessDelegate.Value;
            return item;
        }
    }
}
