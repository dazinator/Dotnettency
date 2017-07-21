using Dotnettency.Container;
using StructureMap;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class StructureMapTenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
    {
        public StructureMapTenantContainerBuilder(IContainer container, Action<TTenant, ConfigurationExpression> configure)
        {
            // Ensure.Argument.NotNull(container, nameof(container));
            // Ensure.Argument.NotNull(configure, nameof(configure));

            Container = container;
            Configure = configure;
        }

        protected IContainer Container { get; }
        protected Action<TTenant, ConfigurationExpression> Configure { get; }

        public virtual Task<ITenantContainerAdaptor> BuildAsync(TTenant tenant)
        {
            // Ensure.Argument.NotNull(tenant, nameof(tenant));

            var tenantContainer = Container.CreateChildContainer();
            tenantContainer.Configure(config => Configure(tenant, config));
            ITenantContainerAdaptor adaptor = new StructureMapTenantContainerAdaptor(tenantContainer);
            return Task.FromResult(adaptor);
        }
    }


}