using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public interface ITenantRequestContainerAccessor<TTenant>
        where TTenant : class
    {
        Lazy<Task<PerRequestContainer>> TenantRequestContainer { get; }
    }
}
