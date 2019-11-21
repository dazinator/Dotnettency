using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellItemFactory<TTenant, TItem>
   where TTenant : class
    {
        Task<TItem> Create(IServiceProvider serviceProviderOverride, TTenant tenant);
    }

}
