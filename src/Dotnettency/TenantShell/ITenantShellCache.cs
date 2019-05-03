using System;

namespace Dotnettency
{
    public interface ITenantShellCache<TTenant>
        where TTenant : class
    {
        TenantShell<TTenant> AddOrUpdate(TenantIdentifier key, Func<TenantIdentifier, TenantShell<TTenant>> addValueFactory, Func<TenantIdentifier, TenantShell<TTenant>, TenantShell<TTenant>> updateValueFactory);

        TenantShell<TTenant> AddOrUpdate(TenantIdentifier key, TenantShell<TTenant> addValue, Func<TenantIdentifier, TenantShell<TTenant>, TenantShell<TTenant>> updateValueFactory);

        bool TryGetValue(TenantIdentifier key, out TenantShell<TTenant> value);
    }    
}
