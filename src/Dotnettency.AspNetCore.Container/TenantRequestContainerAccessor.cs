using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Dotnettency.Container;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.AspNetCore.Container
{
    public class TenantRequestContainerAccessor<TTenant> : ITenantRequestContainerAccessor<TTenant>
        where TTenant : class
    {
        private readonly ITenantContainerAccessor<TTenant> _tenantContainerAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TenantRequestContainerAccessor<TTenant>> _logger;
        private readonly ITenantContainerEventsPublisher<TTenant> _containerEventsPublisher;     

        public TenantRequestContainerAccessor(
            IHttpContextAccessor httpContextAccessor,
            ILogger<TenantRequestContainerAccessor<TTenant>> logger,
            ITenantContainerAccessor<TTenant> tenantContainerAccessor,
            ITenantContainerEventsPublisher<TTenant> containerEventsPublisher)
        {
            _httpContextAccessor = httpContextAccessor;
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

                var requestId = _httpContextAccessor.HttpContext.TraceIdentifier;
                var requestContainer = tenantContainer.CreateNestedContainer($"{tenantContainer.ContainerName} - Request {requestId}");
                logger.LogDebug("Creating container name: {ContainerName} and RequestId: {RequestId}", tenantContainer.ContainerName, requestId);

                _containerEventsPublisher?.PublishNestedTenantContainerCreated(requestContainer);
                return new PerRequestContainer(requestContainer);
            });
        }

        public Lazy<Task<PerRequestContainer>> TenantRequestContainer { get; private set; }
    }
}