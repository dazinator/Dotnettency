using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellResolver<TTenant>
        where TTenant : class
    {
        Task<TenantShell<TTenant>> ResolveTenant(TenantDistinguisher identifier, ITenantShellFactory<TTenant> tenantFactory);
    }
}
