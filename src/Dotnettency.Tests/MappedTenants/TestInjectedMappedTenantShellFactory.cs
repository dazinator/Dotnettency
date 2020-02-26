using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Tests
{
    public class TestInjectedMappedTenantShellFactory : MappedTenantShellFactory<Tenant, int>
    {

        public TestInjectedMappedTenantShellFactory(ILogger<TestInjectedMappedTenantShellFactory> someDependency)
        {
            if (someDependency == null)
            {
                throw new ArgumentNullException(nameof(someDependency));
            }
        }

        protected override Task<Tenant> GetTenant(int key)
        {

            // If we want to have more info about the tenant available in our app than just its key,
            // then in a real system we might do an async lookup to the database or a distributed cache here based on the key.
            // The key comes from our configured mapping options.
            // Note: This method will only be invoked once when the tenant is initialised / or restarted (not on every request).
            if (key == 1)
            {
                return Task.FromResult(new Tenant() { Id = key, Name = "Test Tenant" });
            }

            return Task.FromResult<Tenant>(null); // key does not match a recognised tenant.
        }
    }
}




