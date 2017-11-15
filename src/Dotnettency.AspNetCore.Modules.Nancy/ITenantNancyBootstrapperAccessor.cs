using System;
using System.Threading.Tasks;

namespace Dotnettency.AspNetCore.Modules.Nancy
{
    public interface ITenantNancyBootstrapperAccessor<TTenant>
      where TTenant : class
    {
        Lazy<Task<TenantContainerNancyBootstrapper<TTenant>>> Bootstrapper { get; }
    }
}