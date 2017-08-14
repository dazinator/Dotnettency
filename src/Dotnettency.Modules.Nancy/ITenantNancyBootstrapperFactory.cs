using Dotnettency.Modules.Nancy;
using System.Threading.Tasks;

namespace Dotnettency.Modules.Nancy
{
    public interface ITenantNancyBootstrapperFactory<TTenant>
      where TTenant : class
    {
        Task<TenantContainerNancyBootstrapper<TTenant>> Get(TTenant currentTenant);
    }
}