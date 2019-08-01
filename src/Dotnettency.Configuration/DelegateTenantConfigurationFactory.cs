using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Configuration
{
    public class DelegateTenantConfigurationFactory<TTenant> : ITenantConfigurationFactory<TTenant, IConfigurationBuilder, IConfiguration>
        where TTenant : class
    {
        private readonly Func<TenantConfigurationBuilderContext<TTenant>, IConfigurationBuilder> _buildTenantConfigurationAction;

        public DelegateTenantConfigurationFactory(Func<TenantConfigurationBuilderContext<TTenant>, IConfigurationBuilder> buildTenantConfigurationAction)
        {
            _buildTenantConfigurationAction = buildTenantConfigurationAction;
        }

        public async Task<IConfiguration> Create(IConfigurationBuilder appBuilder, IServiceProvider serviceProviderOverride, TTenant tenant)
        {
            return await CreateTenantConfiguration(appBuilder, serviceProviderOverride, tenant);
        }

        protected virtual Task<IConfigurationRoot> CreateTenantConfiguration(IConfigurationBuilder rootApp, IServiceProvider serviceProviderOverride, TTenant tenant)
        {
            return Task.Run(() =>
            {
                var builderContext = new TenantConfigurationBuilderContext<TTenant>
                {
                    Tenant = tenant
                };
             
                var tenantConfiguBuilder =_buildTenantConfigurationAction(builderContext);
                return tenantConfiguBuilder.Build();
            });
        }
    }
}
