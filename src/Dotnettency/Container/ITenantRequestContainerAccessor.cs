using Dotnettency.Container;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public interface ITenantRequestContainerAccessor<TTenant>
        where TTenant : class
    {
        Lazy<Task<ITenantContainerAdaptor>> TenantRequestContainer { get; }
    }
}
