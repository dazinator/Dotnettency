using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantDistinguisherFactory<TTenant>
        where TTenant : class
    {
        Task<TenantDistinguisher> IdentifyContext();
    }
}
