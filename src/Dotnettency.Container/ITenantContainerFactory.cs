using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public interface ITenantContainerFactory<TTenant>
        where TTenant : class
    {
        Task<ITenantContainerAdaptor> Get(TTenant currentTenant);
    }
}
