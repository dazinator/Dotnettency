using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerBuilderFactory<TTenant> : TenantContainerFactory<TTenant>
        where TTenant : class
    {
        private readonly IServiceProvider _serviceProvider;

        public TenantContainerBuilderFactory(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<ITenantContainerAdaptor> BuildContainer(TenantShellItemBuilderContext<TTenant> currentTenant)
        {
            // If no explicit scoped services provided, then fallback to this current scope.
            if(currentTenant.Services == null)
            {
                currentTenant.Services = _serviceProvider;
            }
            var builder = _serviceProvider.GetRequiredService<ITenantContainerBuilder<TTenant>>();           
            return await builder.BuildAsync(currentTenant);
        }
    }
}
