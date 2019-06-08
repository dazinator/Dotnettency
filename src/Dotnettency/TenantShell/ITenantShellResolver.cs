using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellResolver<TTenant>
        where TTenant : class
    {
        Task<TenantShell<TTenant>> ResolveTenantShell(TenantIdentifier identifier, ITenantShellFactory<TTenant> tenantFactory);

        Task<IDisposable> RemoveTenantShell(TenantIdentifier identifier);
    }
}
