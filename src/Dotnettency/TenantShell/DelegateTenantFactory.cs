using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class DelegateTenantFactory<TTenant> : TenantFactory<TTenant>
    where TTenant : class
    {
        private readonly Func<TenantIdentifier, Task<TTenant>> _getTenant;

        public DelegateTenantFactory(Func<TenantIdentifier, Task<TTenant>> getTenant)
        {
            if (getTenant == null)
            {
                throw new ArgumentNullException(nameof(getTenant));
            }
            _getTenant = getTenant;
        }
        public override Task<TTenant> GetTenant(TenantIdentifier key)
        {
            return _getTenant(key);
        }
    }

}
