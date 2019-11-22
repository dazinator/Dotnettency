using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class DelegateTaskTenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
    where TTenant : class
    {
        private readonly IServiceCollection _defaultServices;
        private readonly ITenantContainerAdaptor _parentContainer;
        private readonly Func<TenantShellItemBuilderContext<TTenant>, IServiceCollection, Task> _configureTenant;
        private readonly ITenantContainerEventsPublisher<TTenant> _containerEventsPublisher;

        public DelegateTaskTenantContainerBuilder(IServiceCollection defaultServices,
            ITenantContainerAdaptor parentContainer,
            Func<TenantShellItemBuilderContext<TTenant>, IServiceCollection, Task> configureTenant,
            ITenantContainerEventsPublisher<TTenant> containerEventsPublisher)
        {
            _defaultServices = defaultServices;
            _parentContainer = parentContainer;
            _configureTenant = configureTenant;
            _containerEventsPublisher = containerEventsPublisher;
        }

        public async Task<ITenantContainerAdaptor> BuildAsync(TTenant tenant)
        {
            var tenantContainer = await _parentContainer.CreateChildContainerAndConfigureAsync("Tenant: " + (tenant?.ToString() ?? "NULL").ToString(),
                async config =>
            {
                // add default services to tenant container.
                // see https://github.com/aspnet/AspNetCore/issues/10469 and issues linked with that.
                if (_defaultServices != null)
                {
                    foreach (var item in _defaultServices)
                    {
                        config.Add(item);
                    }
                }

                var buildContext = new TenantShellItemBuilderContext<TTenant>()
                {
                    Services = _parentContainer,
                    Tenant = tenant
                };

                await _configureTenant(buildContext, config);
            });

            _containerEventsPublisher?.PublishTenantContainerCreated(tenantContainer);
            return tenantContainer;
        }
    }

}
