using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantShellAccessor<TTenant> : ITenantShellAccessor<TTenant>
        where TTenant : class
    {
        private readonly ITenantShellFactory<TTenant> _tenantFactory;
        private readonly TenantIdentifierAccessor<TTenant> _tenantDistinguisherAccessor;
        private readonly ITenantShellResolver<TTenant> _tenantResolver;

        public TenantShellAccessor(ITenantShellFactory<TTenant> tenantFactory,
            TenantIdentifierAccessor<TTenant> tenantDistinguisherAccessor,
            ITenantShellResolver<TTenant> tenantResolver)
        {
            _tenantFactory = tenantFactory;
            _tenantDistinguisherAccessor = tenantDistinguisherAccessor;
            _tenantResolver = tenantResolver;

            CurrentTenantShell = new Lazy<Task<TenantShell<TTenant>>>(async () =>
            {
                var identifier = await _tenantDistinguisherAccessor.TenantDistinguisher.Value;
                if (identifier == null)
                {
                    return null;
                }

                return await _tenantResolver.ResolveTenantShell(identifier, _tenantFactory);
            });
        }

        public Lazy<Task<TenantShell<TTenant>>> CurrentTenantShell { get; private set; }

    }
}
