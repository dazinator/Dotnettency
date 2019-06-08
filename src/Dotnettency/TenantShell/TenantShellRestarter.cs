using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantShellRestarter<TTenant> : ITenantShellRestarter<TTenant>
        where TTenant : class
    {
        private readonly IHttpContextProvider _httpContextProvider;
        private readonly TenantIdentifierAccessor<TTenant> _tenantDistinguisherAccessor;
        private readonly ITenantShellResolver<TTenant> _tenantResolver;

        public TenantShellRestarter(
            IHttpContextProvider httpContextProvider,
            TenantIdentifierAccessor<TTenant> tenantDistinguisherAccessor,
            ITenantShellResolver<TTenant> tenantResolver)
        {
            _httpContextProvider = httpContextProvider;
            _tenantDistinguisherAccessor = tenantDistinguisherAccessor;
            _tenantResolver = tenantResolver;
        }

        public async Task Restart()
        {
            var identifier = await _tenantDistinguisherAccessor.TenantDistinguisher.Value;
            if (identifier == null)
            {
                // current tenant cannot be identified.
                return;
            }

            var disposable = await _tenantResolver.RemoveTenantShell(identifier);
            _httpContextProvider.GetCurrent().SetItem(Guid.NewGuid().ToString(), disposable, true);

        }
    }
}
