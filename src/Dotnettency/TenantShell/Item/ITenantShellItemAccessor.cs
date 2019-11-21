using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellItemAccessor<TTenant, TItem>
         where TTenant : class
    {
        Func<IServiceProvider, Lazy<Task<TItem>>> Factory { get; }
    }

}
