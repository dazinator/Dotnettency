using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantShellAccessor<TTenant> : ITenantShellAccessor<TTenant>
        where TTenant : class
    {
        private readonly ITenantShellFactoryStrategy<TTenant> _tenantFactoryStrategy;
        private readonly TenantIdentifierAccessor<TTenant> _tenantDistinguisherAccessor;
        private readonly ITenantShellResolver<TTenant> _tenantResolver;

        public TenantShellAccessor(ITenantShellFactoryStrategy<TTenant> tenantFactoryStrategy,
            TenantIdentifierAccessor<TTenant> tenantDistinguisherAccessor,
            ITenantShellResolver<TTenant> tenantResolver)
        {
            _tenantFactoryStrategy = tenantFactoryStrategy;
            _tenantDistinguisherAccessor = tenantDistinguisherAccessor;
            _tenantResolver = tenantResolver;

            CurrentTenantShell = new Lazy<Task<TenantShell<TTenant>>>(async () =>
            {
                var identifier = await _tenantDistinguisherAccessor.TenantDistinguisher.Value;
                if (identifier == null)
                {
                    return null;
                }
                var tenantShellFactory = _tenantFactoryStrategy.GetTenantShellFactory();
                return await _tenantResolver.ResolveTenantShell(identifier, tenantShellFactory);
            });
        }

        public Lazy<Task<TenantShell<TTenant>>> CurrentTenantShell { get; private set; }

    }
}
