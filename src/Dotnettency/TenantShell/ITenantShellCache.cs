using System;

namespace Dotnettency
{
    public interface ITenantShellCache<TTenant>
        where TTenant : class
    {
        TenantShell<TTenant> AddOrUpdate(TenantDistinguisher key, Func<TenantDistinguisher, TenantShell<TTenant>> addValueFactory, Func<TenantDistinguisher, TenantShell<TTenant>, TenantShell<TTenant>> updateValueFactory);

        TenantShell<TTenant> AddOrUpdate(TenantDistinguisher key, TenantShell<TTenant> addValue, Func<TenantDistinguisher, TenantShell<TTenant>, TenantShell<TTenant>> updateValueFactory);

        bool TryGetValue(TenantDistinguisher key, out TenantShell<TTenant> value);
    }    
}
