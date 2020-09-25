using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class DelegateActionTenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
        where TTenant : class
    {
       // private readonly IServiceCollection _defaultServices;
        private readonly IServiceCollection _parentServices;

        private readonly ITenantContainerAdaptor _parentContainer;
        private readonly Action<TenantShellItemBuilderContext<TTenant>, IServiceCollection> _configureTenant;
        private readonly ITenantContainerEventsPublisher<TTenant> _containerEventsPublisher;

        public DelegateActionTenantContainerBuilder(
            IServiceCollection parentServices,
            ITenantContainerAdaptor parentContainer,
            Action<TenantShellItemBuilderContext<TTenant>, IServiceCollection> configureTenant,
            ITenantContainerEventsPublisher<TTenant> containerEventsPublisher)
        {
            _parentServices = parentServices;
            _parentContainer = parentContainer;
            _configureTenant = configureTenant;
            _containerEventsPublisher = containerEventsPublisher;
        }

        public Task<ITenantContainerAdaptor> BuildAsync(TenantShellItemBuilderContext<TTenant> tenantContext)
        {
            var name = tenantContext.Tenant?.ToString();
            var tenantContainer = _parentContainer.CreateChildContainerAndConfigure("Tenant: " + (name ?? "NULL"), _parentServices, config =>
             {
                 // add default services to tenant container.
                 // see https://github.com/aspnet/AspNetCore/issues/10469 and issues linked with that.
                 //if (_defaultServices != null)
                 //{
                 //    foreach (var item in _defaultServices)
                 //    {
                 //        config.Add(item);
                 //    }
                 //}

                 // As tenantContext.Services is provided from current scope, we prefer that.
                 // otherwise fallback to parent scope (i.e this could mean falling back to application services from current scoped services etc).
                 // Note: Not sure if this is necessary as current scope will possiblly always match _parentContainer anyway when no more specific scope in use.
                 if (tenantContext.Services == null)
                 {
                     tenantContext.Services = _parentContainer;
                 }

                 _configureTenant(tenantContext, config);
             });

            _containerEventsPublisher?.PublishTenantContainerCreated(tenantContainer);

            return Task.FromResult(tenantContainer);
        }
    }

}
