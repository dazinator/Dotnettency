using System;
using System.Collections.Concurrent;

namespace Dotnettency
{
    public class ConcurrentDictionaryTenantShellCache<TTenant> : ITenantShellCache<TTenant>
        where TTenant : class
    {
        private readonly ConcurrentDictionary<TenantIdentifier, TenantShell<TTenant>> _mappings;

        public ConcurrentDictionaryTenantShellCache()
        {
            _mappings = new ConcurrentDictionary<TenantIdentifier, TenantShell<TTenant>>();
        }

        public TenantShell<TTenant> AddOrUpdate(TenantIdentifier key,
            Func<TenantIdentifier, TenantShell<TTenant>> addValueFactory,
            Func<TenantIdentifier, TenantShell<TTenant>, TenantShell<TTenant>> updateValueFactory)
        {
            return _mappings.AddOrUpdate(key, addValueFactory, updateValueFactory);
        }

        public TenantShell<TTenant> AddOrUpdate(TenantIdentifier key,
            TenantShell<TTenant> addValue,
            Func<TenantIdentifier, TenantShell<TTenant>, TenantShell<TTenant>> updateValueFactory)
        {
            return _mappings.AddOrUpdate(key, addValue, updateValueFactory);
        }

        public bool TryGetValue(TenantIdentifier key, out TenantShell<TTenant> value)
        {
            return _mappings.TryGetValue(key, out value);
        }
    }
}
