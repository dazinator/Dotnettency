using System;
using System.Collections.Concurrent;

namespace Dotnettency
{
    public class ConcurrentDictionaryTenantShellCache<TTenant> : ITenantShellCache<TTenant>
        where TTenant : class
    {
        private readonly ConcurrentDictionary<TenantDistinguisher, TenantShell<TTenant>> _mappings;

        public ConcurrentDictionaryTenantShellCache()
        {
            _mappings = new ConcurrentDictionary<TenantDistinguisher, TenantShell<TTenant>>();
        }

        public TenantShell<TTenant> AddOrUpdate(TenantDistinguisher key, Func<TenantDistinguisher, TenantShell<TTenant>> addValueFactory, Func<TenantDistinguisher, TenantShell<TTenant>, TenantShell<TTenant>> updateValueFactory)
        {
            return _mappings.AddOrUpdate(key, addValueFactory, updateValueFactory);
        }

        public TenantShell<TTenant> AddOrUpdate(TenantDistinguisher key, TenantShell<TTenant> addValue, Func<TenantDistinguisher, TenantShell<TTenant>, TenantShell<TTenant>> updateValueFactory)
        {
            return _mappings.AddOrUpdate(key, addValue, updateValueFactory);
        }

        public bool TryGetValue(TenantDistinguisher key, out TenantShell<TTenant> value)
        {
            return _mappings.TryGetValue(key, out value);
        }
    }
}
