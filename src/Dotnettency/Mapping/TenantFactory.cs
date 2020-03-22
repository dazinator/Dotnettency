using System.Threading.Tasks;

namespace Dotnettency
{   

    public abstract class TenantFactory<TTenant, TKey> : TenantFactory<TTenant>
         where TTenant : class
    {
        private static readonly Task<TTenant> _defaultNullTenant = Task.FromResult(default(TTenant));

        public override Task<TTenant> GetTenant(TenantIdentifier identifier)
        {
            if (identifier.TryGetMappedTenantKey<TKey>(out TKey value))
            {
                return GetTenant(value);
            }
            else
            {
                // could potentially allow behaviour here to be changed in future?
                // depending upon if you want a null tenant or a fallback instance to be returned?
                return _defaultNullTenant;
            }
        }

        public abstract Task<TTenant> GetTenant(TKey value);
    }
}