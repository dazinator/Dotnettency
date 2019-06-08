using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public interface ITenantContainerEvents<TTenant>
        where TTenant : class
    {      
        event Action<Task<TTenant>, IServiceProvider> TenantContainerCreated;
        event Action<Task<TTenant>, IServiceProvider> NestedTenantContainerCreated;     
    }
}