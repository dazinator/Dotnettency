using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class TenantContainerBuilderFactory<TTenant> : TenantContainerFactory<TTenant>
     where TTenant : class
    {

        private readonly IServiceProvider _serviceProvider;

        public TenantContainerBuilderFactory(IServiceProvider servicePrvider)
            : base(servicePrvider)
        {
            _serviceProvider = servicePrvider;
        }

        protected override async Task<ITenantContainerAdaptor> BuildContainer(TTenant currentTenant)
        {
            var builder = _serviceProvider.GetRequiredService<ITenantContainerBuilder<TTenant>>();
            var container = await builder.BuildAsync(currentTenant);
            return container;
        }
    }
}
