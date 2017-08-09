using System;
using System.Threading.Tasks;
using Dotnettency.Modules.Nancy;

namespace Dotnettency
{
    public static class TenantShellNancyExtensions
    {
        public static Lazy<Task<TenantContainerNancyBootstrapper<TTenant>>> GetOrAddNancyBootstrapper<TTenant>(this TenantShell<TTenant> tenantShell, Func<Task<TenantContainerNancyBootstrapper<TTenant>>> nancyBootstrapper)
            where TTenant : class
        {
            var result = tenantShell.Properties.GetOrAdd(nameof(TenantShellNancyExtensions),
                (a) =>
                {
                    //  var factory = containerAdaptorFactory();
                    return new Lazy<Task<TenantContainerNancyBootstrapper<TTenant>>>(nancyBootstrapper);
                }) as Lazy<Task<TenantContainerNancyBootstrapper<TTenant>>>;
            return result;
        }
     
    }
}