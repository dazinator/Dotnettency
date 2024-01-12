using Dotnettency.Container;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{

    public class TenantRequestContainerAccessor<TTenant> : ITenantRequestContainerAccessor<TTenant>
     where TTenant : class
    {
        private readonly ITenantContainerEventsPublisher<TTenant> _containerEventsPublisher;

        public TenantRequestContainerAccessor(
            ILogger<TenantRequestContainerAccessor<TTenant>> logger,
            ITenantContainerAccessor<TTenant> tenantContainerAccessor,
            ITenantContainerEventsPublisher<TTenant> containerEventsPublisher)
        {
            _containerEventsPublisher = containerEventsPublisher;

            TenantRequestContainer = new Lazy<Task<ITenantContainerAdaptor>>(async () =>
            {
                var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
                if (tenantContainer == null)
                {
                    return null;
                }

                var requestContainer = tenantContainer.CreateScope($"{tenantContainer.ContainerName}");
                logger.LogDebug("Creating container name: {ContainerName}", tenantContainer.ContainerName);

                _containerEventsPublisher?.PublishNestedTenantContainerCreated(requestContainer);
                return requestContainer;
            });
        }

        public Lazy<Task<ITenantContainerAdaptor>> TenantRequestContainer { get; private set; }
    }
}
