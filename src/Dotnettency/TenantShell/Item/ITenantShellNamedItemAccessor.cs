using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellNamedItemAccessor<TTenant, TItem>
    {
        Func<IServiceProvider, string, Lazy<Task<TItem>>> NamedFactory { get; }
    }

}
