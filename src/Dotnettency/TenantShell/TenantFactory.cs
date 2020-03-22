using System.Threading.Tasks;

namespace Dotnettency
{
    public abstract class TenantFactory<TTenant>
       where TTenant : class
    {
        public abstract Task<TTenant> GetTenant(TenantIdentifier identifier);
    }

}
