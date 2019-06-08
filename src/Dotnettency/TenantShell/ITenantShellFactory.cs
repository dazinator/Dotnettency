using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantShellFactory<TTenant>
        where TTenant : class
    {
        Task<TenantShell<TTenant>> Get(TenantIdentifier identifier);
    }
}
