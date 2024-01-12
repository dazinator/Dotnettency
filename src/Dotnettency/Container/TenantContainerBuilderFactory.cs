using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerBuilderFactory<TTenant> : TenantContainerFactory<TTenant>
        where TTenant : class
    {
        private readonly ITenantContainerAdaptor _serviceProvider;

        public TenantContainerBuilderFactory(ITenantContainerAdaptor serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<ITenantContainerAdaptor> BuildContainer(TenantShellItemBuilderContext<TTenant> currentTenant)
        {
            // If no explicit scoped services provided, then fallback to this current scope.
            var sp = currentTenant.Services ?? _serviceProvider;
            if(currentTenant.Services == null)
            {
                currentTenant.Services = sp;
            }
            var builder = sp.GetRequiredService<ITenantContainerBuilder<TTenant>>();           
            return await builder.BuildAsync(currentTenant);
        }
    }
}
