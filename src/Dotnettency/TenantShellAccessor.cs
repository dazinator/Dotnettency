using System;
using System.Threading.Tasks;

namespace Dotnettency
{

    public class TenantShellAccessor<TTenant> : ITenantShellAccessor<TTenant>
         where TTenant : class
    {

        private readonly ITenantShellFactory<TTenant> _tenantFactory;
        private readonly TenantDistinguisherAccessor<TTenant> _tenantDistinguisherAccessor;
        private readonly ITenantShellResolver<TTenant> _tenantResolver;

        public TenantShellAccessor(ITenantShellFactory<TTenant> tenantFactory,
            TenantDistinguisherAccessor<TTenant> tenantDistinguisherAccessor,
            ITenantShellResolver<TTenant> tenantResolver)
        {
            _tenantFactory = tenantFactory;
            _tenantDistinguisherAccessor = tenantDistinguisherAccessor;
            _tenantResolver = tenantResolver;

            CurrentTenantShell = new Lazy<Task<TenantShell<TTenant>>>(async () =>
            {
                // return new Task<TTenant>(async () =>
                //{
                var identifier = await _tenantDistinguisherAccessor.TenantDistinguisher.Value;
                if (identifier == null)
                {
                    return null;
                }

                // grab the tenant id for the context id.
                // if we don't have one then 
                var tenantShell = await _tenantResolver.ResolveTenant(identifier, _tenantFactory);
                return tenantShell;
            });
        }

        public Lazy<Task<TenantShell<TTenant>>> CurrentTenantShell { get; }

    }
}
