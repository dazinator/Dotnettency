using Dazinator.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{

    public class NamedFactoryTenantShellFactory<TTenant> : ITenantShellFactory<TTenant>
        where TTenant : class
    {
        private static readonly TenantShell<TTenant> _defaultNullShell = default(TenantShell<TTenant>);
        private readonly TenantFactory<TTenant> _defaultFactory;
        private readonly NamedServiceResolver<TenantFactory<TTenant>> _namedFactories;

        public NamedFactoryTenantShellFactory(TenantFactory<TTenant> defaultFactory, NamedServiceResolver<TenantFactory<TTenant>> namedFactories)
        {
            if (defaultFactory == null)
            {
                throw new ArgumentNullException(nameof(defaultFactory));
            }

            _defaultFactory = defaultFactory;
            _namedFactories = namedFactories;
            //  _serviceProvider = serviceProvider;
        }

        public async Task<TenantShell<TTenant>> Get(TenantIdentifier identifier)
        {
            TTenant tenant = null;

            TenantFactory<TTenant> factory = _defaultFactory;
            var factoryName = identifier.FactoryName ?? string.Empty;

            if (!string.IsNullOrEmpty(factoryName))
            {
                factory = _namedFactories?[factoryName];
                if (factory == null)
                {
                    throw new System.Exception($"Tenant factory named: {factoryName} not found.");
                }
            }

            tenant = await factory.GetTenant(identifier);
            var shell = GetTenantShell(identifier, tenant);
            return shell;
        }

        protected virtual TenantShell<TTenant> GetTenantShell(TenantIdentifier identifier, TTenant tenant)
        {
            if (tenant == null)
            {
                // don't return a tenant shell for a null tenant as this could be source of bugs.
                // if a catch-all shell is needed, developer can map a pattern like ** as the last mapping mapped to a tenant key XYZ,
                // and then use that key to return a default TTenant instance.
                return _defaultNullShell;
            }
            return new TenantShell<TTenant>(tenant, identifier);
        }
    }
}