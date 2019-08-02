using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Configuration
{
    public class DelegateTenantConfigurationFactory<TTenant> : ITenantConfigurationFactory<TTenant, IConfiguration>
        where TTenant : class
    {
        private readonly Func<TenantConfigurationBuilderContext<TTenant>, IConfigurationBuilder> _buildTenantConfigurationAction;

        public DelegateTenantConfigurationFactory(Func<TenantConfigurationBuilderContext<TTenant>, IConfigurationBuilder> buildTenantConfigurationAction)
        {
            _buildTenantConfigurationAction = buildTenantConfigurationAction;
        }

        public async Task<IConfiguration> Create(IServiceProvider serviceProviderOverride, TTenant tenant)
        {
            return await CreateTenantConfiguration(serviceProviderOverride, tenant);
        }

        protected virtual Task<IConfigurationRoot> CreateTenantConfiguration(IServiceProvider applicationServices, TTenant tenant)
        {
            return Task.Run(() =>
            {
                var builderContext = new TenantConfigurationBuilderContext<TTenant>
                {
                    Tenant = tenant,
                    ApplicationServices = applicationServices
                };
             
                var tenantConfiguBuilder =_buildTenantConfigurationAction(builderContext);
                return tenantConfiguBuilder.Build();
            });
        }
    }
}
