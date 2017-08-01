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

        private Lazy<Task<ITenantContainerAdaptor>> _containerFactory;

        public TenantContainerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
             ITenantContainerFactory<TTenant> factory)
        {
            _next = next;
            _log = loggerFactory.CreateLogger<TenantContainerMiddleware<TTenant>>();
            _factory = factory;
        }

        public async Task Invoke(HttpContext context, ITenantContainerAccessor<TTenant> tenantContainerAccessor)
        {
            //  log.LogDebug("Using multitenancy provider {multitenancyProvidertype}.", tenantAccessor.GetType().Name);


            var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
            if (tenantContainer == null)
            {
                await _next.Invoke(context);
                return;
            }

            // Replace request services with a nested version (for lifetime management - used to encpasulate a request).
            using (var scope = tenantContainer.CreateNestedContainer())
            {

                _log.LogDebug("Setting Request: {containerId} - {containerName}", scope.ContainerId, scope.ContainerName);
                var oldRequestServices = context.RequestServices;
                context.RequestServices = scope.GetServiceProvider();
                await _next.Invoke(context); // module middleware should be next - which will replace again with module specific container (nested).
                _log.LogDebug("Restoring Request Container");
                context.RequestServices = oldRequestServices;
            }

        }
    }
}