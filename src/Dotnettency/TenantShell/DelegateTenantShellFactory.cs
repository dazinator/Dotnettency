using System.Threading.Tasks;
using System;

namespace Dotnettency
{
    public class DelegateTenantShellFactory<TTenant> : ITenantShellFactory<TTenant>
        where TTenant : class
    {
        private readonly Func<TenantIdentifier, TenantShell<TTenant>> _factory;

        public DelegateTenantShellFactory(Func<TenantIdentifier, TenantShell<TTenant>> factory)
        {
            _factory = factory;
        }

        public Task<TenantShell<TTenant>> Get(TenantIdentifier identifier)
        {
            return Task.Run(() =>
            {
                return _factory(identifier);
            });
        }
        //}
    }
}