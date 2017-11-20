using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dotnettency.Container;

namespace Dotnettency.AspNetCore.Container
{
    public class TenantRequestContainerAccessor<TTenant> : ITenantRequestContainerAccessor<TTenant>
        where TTenant : class
    {
        private readonly ITenantContainerAccessor<TTenant> _tenantContainerAccessor;
        private readonly ILogger<TenantRequestContainerAccessor<TTenant>> _logger;
        private readonly ITenantContainerEventsPublisher<TTenant> _containerEventsPublisher;     

        public TenantRequestContainerAccessor(
            ILogger<TenantRequestContainerAccessor<TTenant>> logger,
            ITenantContainerAccessor<TTenant> tenantContainerAccessor,
            ITenantContainerEventsPublisher<TTenant> containerEventsPublisher)
        {
            _logger = logger;
            _tenantContainerAccessor = tenantContainerAccessor;
            _containerEventsPublisher = containerEventsPublisher;          

            TenantRequestContainer = new Lazy<Task<PerRequestContainer>>(async () =>
            {
                var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
                if (tenantContainer == null)
                {
                    return null;
                }

                var requestContainer = tenantContainer.CreateNestedContainer();
                _containerEventsPublisher?.PublishNestedTenantContainerCreated(requestContainer);
                return new PerRequestContainer(requestContainer);
            });
        }

        public Lazy<Task<PerRequestContainer>> TenantRequestContainer { get; private set; }
    }
}