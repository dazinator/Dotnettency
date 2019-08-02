using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Configuration
{
    public class TenantConfigurationAccessor<TTenant> : ITenantConfigurationAccessor<TTenant, IConfiguration>
     where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;
        private readonly ITenantConfigurationFactory<TTenant, IConfiguration> _tenantConfigFactory;


        public TenantConfigurationAccessor(ITenantShellAccessor<TTenant> tenantShellAccessor, ITenantConfigurationFactory<TTenant, IConfiguration> tenantConfigFactory)
        {
            _tenantShellAccessor = tenantShellAccessor;
            _tenantConfigFactory = tenantConfigFactory;

            // takes a builder to build the tenants config with, also takes the system level service provider, and system level IConfiguration so
            // that system services can be used when deciding how to build this tenants config.
            // note, building a tenant config does not take any dependency upon tenant sevices or tenant middleware - as assumedly - tenant config would often be
            // needed as a dependency for building tenant services and tenant middleware - and we don't want a circular dependency.
            ConfigFactory = new Func<IServiceProvider, Lazy<Task<IConfiguration>>>((sp) =>
            {
                return new Lazy<Task<IConfiguration>>(async () =>
                {
                    var tenantShell = await _tenantShellAccessor.CurrentTenantShell.Value;
                    if (tenantShell == null)
                    {
                        // no tenant shell - return application level configuration.
                        return (IConfiguration)sp.GetService(typeof(IConfiguration));
                    }

                    var tenant = tenantShell?.Tenant;
                    var tenantPipeline = tenantShell.GetOrAddConfiguration(new Lazy<Task<IConfiguration>>(() =>
                    {
                        return _tenantConfigFactory.Create(sp, tenant);
                    }));

                    return await tenantPipeline.Value;
                });
            });
        }

        public Func<IServiceProvider, Lazy<Task<IConfiguration>>> ConfigFactory { get; private set; }
    }

}
