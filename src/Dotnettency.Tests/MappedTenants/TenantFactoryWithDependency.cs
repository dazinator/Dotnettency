using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Tests
{
    public class TenantFactoryWithDependency : TenantFactory<Tenant, int>
    {

        public TenantFactoryWithDependency(ILogger<TenantFactoryWithDependency> someDependency)
        {
            if (someDependency == null)
            {
                throw new ArgumentNullException(nameof(someDependency));
            }
        }

        public override Task<Tenant> GetTenant(int key)
        {
            // during system setup just return the special tenant.
            return Task.FromResult(new Tenant() { Id = key, Name = nameof(TenantFactoryWithDependency), IsSystemSetup = true });
        }
    }
}




