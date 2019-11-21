using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class DelegateActionTenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
        where TTenant : class
    {
        private readonly IServiceCollection _defaultServices;
        private readonly ITenantContainerAdaptor _parentContainer;
        private readonly Action<TenantShellItemBuilderContext<TTenant>, IServiceCollection> _configureTenant;
        private readonly ITenantContainerEventsPublisher<TTenant> _containerEventsPublisher;

        public DelegateActionTenantContainerBuilder(IServiceCollection defaultServices,
            ITenantContainerAdaptor parentContainer,
            Action<TenantShellItemBuilderContext<TTenant>, IServiceCollection> configureTenant,
            ITenantContainerEventsPublisher<TTenant> containerEventsPublisher)
        {
            _defaultServices = defaultServices;
            _parentContainer = parentContainer;
            _configureTenant = configureTenant;
            _containerEventsPublisher = containerEventsPublisher;
        }

        public Task<ITenantContainerAdaptor> BuildAsync(TTenant tenant)
        {
            var tenantContainer = _parentContainer.CreateChildContainerAndConfigure("Tenant: " + (tenant?.ToString() ?? "NULL").ToString(), config =>
             {
                 // add default services to tenant container.
                 // see https://github.com/aspnet/AspNetCore/issues/10469 and issues linked with that.
                 if(_defaultServices!=null)
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

                 _configureTenant(buildContext, config);
             });

            // tenantContainer.Configure();

            _containerEventsPublisher?.PublishTenantContainerCreated(tenantContainer);

            return Task.FromResult(tenantContainer);
        }
    }

}
