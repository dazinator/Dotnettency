using System.Threading.Tasks;

namespace Dotnettency.AspNetCore.Modules.Nancy
{
    public interface ITenantNancyBootstrapperFactory<TTenant>
      where TTenant : class
    {
        Task<TenantContainerNancyBootstrapper<TTenant>> Get(TTenant currentTenant);
    }
}