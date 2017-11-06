using System.Threading.Tasks;
using System;

namespace Dotnettency
{
    public class DelegateTenantShellFactory<TTenant> : ITenantShellFactory<TTenant>
        where TTenant : class
    {
        private readonly Func<TenantDistinguisher, TenantShell<TTenant>> _factory;

        public DelegateTenantShellFactory(Func<TenantDistinguisher, TenantShell<TTenant>> factory)
        {
            _factory = factory;
        }

        public Task<TenantShell<TTenant>> Get(TenantDistinguisher identifier)
        {
            return Task.Run(() =>
            {
                return _factory(identifier);
            });
        }
    }
}
