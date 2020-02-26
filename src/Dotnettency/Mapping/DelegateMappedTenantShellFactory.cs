using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class DelegateMappedTenantShellFactory<TTenant, TKey> : MappedTenantShellFactory<TTenant, TKey>
        where TTenant : class
    {
        private readonly Func<TKey, Task<TTenant>> _getTenant;

        public DelegateMappedTenantShellFactory(Func<TKey, Task<TTenant>> getTenant)
        {
            if (getTenant == null)
            {
                throw new ArgumentNullException(nameof(getTenant));
            }
            _getTenant = getTenant;
        }
        protected override Task<TTenant> GetTenant(TKey key)
        {
            return _getTenant(key);
        }
    }
}