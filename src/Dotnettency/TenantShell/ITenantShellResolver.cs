using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellResolver<TTenant>
        where TTenant : class
    {
        Task<TenantShell<TTenant>> ResolveTenantShell(TenantIdentifier identifier, Func<ITenantShellFactory<TTenant>> getShellFactory);

        Task<IDisposable> RemoveTenantShell(TenantIdentifier identifier);
    }
}
