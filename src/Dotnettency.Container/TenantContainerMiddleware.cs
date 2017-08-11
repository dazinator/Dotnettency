using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dotnettency.Container
{
    public class TenantContainerMiddleware<TTenant>
         where TTenant : class
    {

        private readonly RequestDelegate _next;
        private readonly ILogger _log;
        private readonly ITenantContainerFactory<TTenant> _factory;

        //  private Lazy<Task<ITenantContainerAdaptor>> _containerFactory;

        public TenantContainerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory
            //  ITenantContainerFactory<TTenant> factory
            )
        {
            _next = next;
            _log = loggerFactory.CreateLogger<TenantContainerMiddleware<TTenant>>();
            //  _factory = factory;
        }

        public async Task Invoke(HttpContext context, ITenantRequestContainerAccessor<TTenant> tenantRequestContainerAccessor)
        {
            //  log.LogDebug("Using multitenancy provider {multitenancyProvidertype}.", tenantAccessor.GetType().Name);

            var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;
            if (requestContainer == null)
            {
                await _next.Invoke(context);
                return;
            }

            // Replace request services with a nested version (for lifetime management - used to encpasulate a request).
            using (requestContainer)
            {
                _log.LogDebug("Setting Request Container: {containerId} - {containerName}", requestContainer.RequestContainer.ContainerId, requestContainer.RequestContainer.ContainerName);
                await requestContainer.ExecuteWithinSwappedRequestContainer(_next.Invoke(context));
            }

        }
    }
}