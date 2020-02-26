namespace Dotnettency
{
    /// <summary>
    /// Used to select an implementation of <see cref="ITenantShellFactory{TTenant}"/> to resolve dynamically during a request, in order to initialise
    /// a tenant's shell.
    /// </summary>
    ///<remarks>
    ///This only occurs when a Tenant needs to be initialised, e.g the server is receiving the first request for the tenant.
    ///</remarks>
    public interface ITenantShellFactoryStrategy<TTenant>
        where TTenant : class
    {
        ITenantShellFactory<TTenant> GetTenantShellFactory(TenantIdentifier identifier);
    }
}
