using System;
using System.Threading.Tasks;

namespace Dotnettency.AspNetCore.Container
{
    public interface ITenantRequestContainerAccessor<TTenant>
        where TTenant : class
    {
        Lazy<Task<PerRequestContainer>> TenantRequestContainer { get; }
    }
}
