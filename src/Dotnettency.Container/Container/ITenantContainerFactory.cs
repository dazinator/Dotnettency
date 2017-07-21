using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public interface ITenantContainerFactory<TTenant>
       where TTenant : class
    {
        // Task<TTenant> Resolve(TenantIdentifier identifier);
        Task<ITenantContainerAdaptor> Get(TTenant currentTenant);
    }


}
