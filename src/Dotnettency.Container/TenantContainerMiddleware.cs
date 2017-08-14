using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency.Container
{
    public class TenantContainerMiddleware<TTenant>
         where TTenant : class
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<TenantContainerMiddleware<TTenant>> _logger;
        private readonly IApplicationBuilder _appBuilder;

        public TenantContainerMiddleware(
            RequestDelegate next,
            ILogger<TenantContainerMiddleware<TTenant>> logger,
            IApplicationBuilder appBuilder)
        {
            _next = next;
            _logger = logger;
            _appBuilder = appBuilder;            
        }

        public async Task Invoke(HttpContext context, ITenantContainerAccessor<TTenant> tenantContainerAccessor, ITenantRequestContainerAccessor<TTenant> requestContainerAccessor)
        {
            //  log.LogDebug("Using multitenancy provider {multitenancyProvidertype}.", tenantAccessor.GetType().Name);
            _logger.LogDebug("Tenant Container Middleware - Start.");
            var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
            if (tenantContainer == null)
            {
                _logger.LogDebug("Tenant Container Middleware - No tenant container.");
                await _next.Invoke(context);
                return;
            }

            IServiceProvider oldAppBuilderServices = _appBuilder.ApplicationServices;
            try
            {
                _logger.LogDebug("Setting AppBuilder Services to Tenant Container: {containerId} - {containerName}", tenantContainer.ContainerId, tenantContainer.ContainerName);
                _appBuilder.ApplicationServices = tenantContainer;
                var perRequestContainer = await requestContainerAccessor.TenantRequestContainer.Value;

                // Replace request services with a nested version (for lifetime management - used to encpasulate a request).                
                using (perRequestContainer)
                {
                    _logger.LogDebug("Setting Request Container: {containerId} - {containerName}", perRequestContainer.RequestContainer.ContainerId, perRequestContainer.RequestContainer.ContainerName);
                    await perRequestContainer.ExecuteWithinSwappedRequestContainer(_next, context);
                    _logger.LogDebug("Restoring Request Container");
                }
            }
            finally
            {
                _logger.LogDebug("Restoring AppBuilder Services");
                _appBuilder.ApplicationServices = oldAppBuilderServices;
            }


        }
    }
}