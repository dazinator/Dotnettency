using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantShellAccessor<TTenant> : ITenantShellAccessor<TTenant>
        where TTenant : class
    {
        //  private readonly ITenantShellFactoryStrategy<TTenant> _tenantFactoryStrategy;
        private readonly TenantIdentifierAccessor<TTenant> _tenantDistinguisherAccessor;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITenantShellResolver<TTenant> _tenantShellResolver;

        public TenantShellAccessor(
            TenantIdentifierAccessor<TTenant> tenantDistinguisherAccessor, ITenantShellResolver<TTenant> shellResolver, IServiceProvider serviceProvider)
        {
            _tenantDistinguisherAccessor = tenantDistinguisherAccessor;
            _serviceProvider = serviceProvider;
            _tenantShellResolver = shellResolver;

            CurrentTenantShell = new Lazy<Task<TenantShell<TTenant>>>(async () =>
            {
                var identifier = await _tenantDistinguisherAccessor.TenantDistinguisher.Value;
                if (identifier == null)
                {
                    return null;
                }

                return await _tenantShellResolver.ResolveTenantShell(identifier, () =>
                {
                    // We don't inject this in the construcotr because we only need this dependency if we have to intiialise a new tenant on a cache miss,
                    // and we have optiised for the typical case - i.e we usually won't have to do this very often.
                    ITenantShellFactory<TTenant> shellFactory = serviceProvider.GetRequiredService<ITenantShellFactory<TTenant>>();
                    return shellFactory;
                });
            });
        }

        public Lazy<Task<TenantShell<TTenant>>> CurrentTenantShell { get; private set; }

    }
}
