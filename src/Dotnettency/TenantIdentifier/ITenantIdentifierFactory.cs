using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantIdentifierFactory<TTenant>
        where TTenant : class
    {
        Task<TenantIdentifier> IdentifyTenant();
    }
}
