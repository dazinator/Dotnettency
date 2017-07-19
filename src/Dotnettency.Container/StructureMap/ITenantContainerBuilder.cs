using StructureMap;
using System.Threading.Tasks;

namespace WebExperiment
{
    public interface ITenantContainerBuilder<TTenant>
    {
        Task<ITenantContainerAdaptor> BuildAsync(TTenant tenant);
    }


}