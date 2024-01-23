using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Dazinator.Extensions.DependencyInjection.ChildContainers;

namespace Dotnettency.Container
{
    public class DelegateTaskTenantContainerBuilder<TTenant> : ITenantContainerBuilder<TTenant>
    where TTenant : class
    {
        private readonly ITenantContainerAdaptor _parentContainer;
        private readonly Func<TenantShellItemBuilderContext<TTenant>, IChildServiceCollection, Task> _configureTenant;
        private readonly ITenantContainerEventsPublisher<TTenant> _containerEventsPublisher;

        public DelegateTaskTenantContainerBuilder(
            ITenantContainerAdaptor parentContainer,
            Func<TenantShellItemBuilderContext<TTenant>, IChildServiceCollection, Task> configureTenant,
            ITenantContainerEventsPublisher<TTenant> containerEventsPublisher)
        {
            _parentContainer = parentContainer;
            _configureTenant = configureTenant;
            _containerEventsPublisher = containerEventsPublisher;
        }

        public async Task<ITenantContainerAdaptor> BuildAsync(TenantShellItemBuilderContext<TTenant> tenantContext)
        {
            var tenantContainer = await _parentContainer.CreateChildAsync("Tenant: " + (tenantContext?.Tenant?.ToString() ?? "NULL").ToString(), async config =>
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
               
                await _configureTenant(tenantContext, config);
            });

            _containerEventsPublisher?.PublishTenantContainerCreated(tenantContainer);
            return tenantContainer;
        }
    }

}
