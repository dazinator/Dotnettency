namespace Dotnettency.Tests
{
    //public class SystemSetupMappedTenantShellFactory : MappedTenantShellFactory<Tenant, int>
    //{

    //    private readonly Task<Tenant> _systemSetupTenant = Task.FromResult(new Tenant() { Id = -1, Name = "System Setup", IsSystemSetup = true });

    //    public SystemSetupMappedTenantShellFactory(ILogger<TestInjectedMappedTenantShellFactory> someDependency)
    //    {
    //        if (someDependency == null)
    //        {
    //            throw new ArgumentNullException(nameof(someDependency));
    //        }
    //    }

    //    protected override Task<Tenant> GetTenant(int key)
    //    {
    //        // during system setup just return the special tenant.
    //        return _systemSetupTenant;
    //    }
    //}
}




