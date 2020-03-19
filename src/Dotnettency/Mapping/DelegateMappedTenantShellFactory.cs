using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class DelegateTenantFactory<TTenant, TKey> : TenantFactory<TTenant, TKey>
        where TTenant : class
    {
        private readonly Func<TKey, Task<TTenant>> _getTenant;

        public DelegateTenantFactory(Func<TKey, Task<TTenant>> getTenant)
        {
            if (getTenant == null)
            {
                throw new ArgumentNullException(nameof(getTenant));
            }
            _getTenant = getTenant;
        }
        public override Task<TTenant> GetTenant(TKey key)
        {
            return _getTenant(key);
        }
    }
}