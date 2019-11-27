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

        public TTenant Tenant
        {
            get;
            set;
        }

        public IServiceProvider Services { get; set; }

        public Task<TItem> GetShellItemAsync<TItem>(string name = "")
        {
            return Services.GetShellItemAsync<TTenant, TItem>(name);
        }
    }
}
