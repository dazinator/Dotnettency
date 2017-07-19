using StructureMap;
using System.Threading.Tasks;

namespace WebExperiment
{
    public interface ITenantContainerFactory<TTenant>
       where TTenant : class
    {
        // Task<TTenant> Resolve(TenantIdentifier identifier);
        Task<ITenantContainerAdaptor> Get(TenantIdentifier identifier);
    }


}
