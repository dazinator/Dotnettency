using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Configuration
{
    public class TenantConfigurationAccessor<TTenant> : ITenantConfigurationAccessor<TTenant, IConfigurationBuilder, IConfiguration>
     where TTenant : class
    {
        private readonly ITenantShellAccessor<TTenant> _tenantShellAccessor;

        public TenantConfigurationAccessor(ITenantShellAccessor<TTenant> tenantShellAccessor)
        {
            _tenantShellAccessor = tenantShellAccessor;

            // takes a builder to build the tenants config with, also takes the system level service provider, and system level IConfiguration so
            // that system services can be used when deciding how to build this tenants config.
            // note, building a tenant config does not take any dependency upon tenant sevices or tenant middleware - as assumedly - tenant config would often be
            // needed as a dependency for building tenant services and tenant middleware - and we don't want a circular dependency.
            BuildTenantConfigDelegate = new Func<IConfigurationBuilder, IServiceProvider, ITenantConfigurationFactory<TTenant, IConfigurationBuilder, IConfiguration>, Lazy<Task<IConfiguration>>>((builder, sp, factory) =>
            {
                return new Lazy<Task<IConfiguration>>(async () =>
                {
                    var tenantShell = await _tenantShellAccessor.CurrentTenantShell.Value;
                    if (tenantShell == null)
                    {
                        // no tenant
                        return (IConfiguration)sp.GetService(typeof(IConfiguration));
                       // return next;
                    }

                    var tenant = tenantShell?.Tenant;
                    var tenantPipeline = tenantShell.GetOrAddConfiguration(new Lazy<Task<IConfiguration>>(() =>
                    {
                        return factory.Create(builder, sp, tenant);
                    }));

                    return await tenantPipeline.Value;
                });
            });
        }

        public Func<IConfigurationBuilder, IServiceProvider, ITenantConfigurationFactory<TTenant, IConfigurationBuilder, IConfiguration>, Lazy<Task<IConfiguration>>> BuildTenantConfigDelegate { get; private set; }
    }

}
