using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellResolver<TTenant>
        where TTenant : class
    {
        Task<TenantShell<TTenant>> ResolveTenant(TenantIdentifier identifier, ITenantShellFactory<TTenant> tenantFactory);
    }
}
