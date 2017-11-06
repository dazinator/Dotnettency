using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerBuilderFactory<TTenant> : TenantContainerFactory<TTenant>
        where TTenant : class
    {
        private readonly IServiceProvider _serviceProvider;

        public TenantContainerBuilderFactory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task<ITenantContainerAdaptor> BuildContainer(TTenant currentTenant)
        {
            var builder = _serviceProvider.GetRequiredService<ITenantContainerBuilder<TTenant>>();
            return await builder.BuildAsync(currentTenant);
        }
    }
}
