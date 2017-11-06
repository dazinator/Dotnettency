using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class TenantAccessor<TTenant> : ITenantAccessor<TTenant>
        where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;

        public TenantAccessor(ITenantShellAccessor<TTenant> tenantShellAccessor)
        {
            _tenantShellAccessor = tenantShellAccessor;

            CurrentTenant = new Lazy<Task<TTenant>>(async () =>
            {
                var tenantShell = await _tenantShellAccessor.CurrentTenantShell?.Value;
                return tenantShell?.Tenant;
            });
        }

        public Lazy<Task<TTenant>> CurrentTenant { get; private set; }
    }
}
