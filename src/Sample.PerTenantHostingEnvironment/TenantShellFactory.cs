using Dotnettency;
using System;
using System.Threading.Tasks;

namespace Sample.PerTenantHostingEnvironment
{
    public class TenantShellFactory : ITenantShellFactory<Tenant>
    {
        public async Task<TenantShell<Tenant>> Get(TenantDistinguisher distinguisher)
        {
            if (distinguisher.Key == "http://localhost:63291")
            {
                Guid tenantId = Guid.Parse("b17fcd22-0db1-47c0-9fef-1aa1cb09605e");
                var tenant = new Tenant(tenantId) { Name = "Foo" };
                var result = new TenantShell<Tenant>(tenant);
                return result;
            }

            if (distinguisher.Key.Contains(":5000") || distinguisher.Key.Contains(":5001"))
            {
                Guid tenantId = Guid.Parse("049c8cc4-3660-41c7-92f0-85430452be22");
                var tenant = new Tenant(tenantId) { Name = "Bar" };
                var result = new TenantShell<Tenant>(tenant, "http://localhost:5000", "http://localhost:5001"); // additional distinguishers to map this same tenant shell instance too.
                return result;
            }

            // for an unknown tenant, we can either create the tenant shell as a NULL tenant by returning a TenantShell<TTenant>(null),
            // which results in the TenantShell being created, and will explicitly have to be reloaded() in order for this method to be called again.                        
            if (distinguisher.Key.Contains("5002"))
            {
                var result = new TenantShell<Tenant>(null);
                return result;
            }

            if (distinguisher.Key.Contains("5003"))
            {

                // or we can return null - which means we wil keep attempting to resolve the tenant on every subsequent request until a result is returned in future.
                // (i.e allows tenant to be created in backend in a few moments time). 
                return null;
            }

            throw new NotImplementedException("Please make request on ports 5000 - 5003 to see various behaviour. Can also use 63291 when launching under IISExpress");

        }
    }
}
