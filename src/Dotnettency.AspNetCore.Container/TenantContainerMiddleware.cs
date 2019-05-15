using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Dotnettency.Container;
using Dotnettency.Middleware;

namespace Dotnettency.AspNetCore.Container
{
    public class TenantContainerMiddleware<TTenant>
        where TTenant : class
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantContainerMiddleware<TTenant>> _logger;
        private readonly AppBuilderAdaptorBase _appBuilder;
        private readonly IHttpContextProvider _httpContextProvider;

        public TenantContainerMiddleware(
            RequestDelegate next,
            ILogger<TenantContainerMiddleware<TTenant>> logger,            
            AppBuilderAdaptorBase appBuilder,
            IHttpContextProvider httpContextProvider
            
            )
        {
            _next = next;
            _logger = logger;
            _appBuilder = appBuilder;
            _httpContextProvider = httpContextProvider;
        }

        public IHttpContextProvider HttpContextProvider => _httpContextProvider;

        public async Task Invoke(HttpContext context,
            RequestServicesSwapper<TTenant> requestServicesSwapper,
            ITenantContainerAccessor<TTenant> tenantContainerAccessor,
            ITenantRequestContainerAccessor<TTenant> requestContainerAccessor)
        {
            _logger.LogDebug("Tenant Container Middleware - Start.");

            var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
            if (tenantContainer == null)
            {
                _logger.LogDebug("Tenant Container Middleware - No tenant container.");
                await _next.Invoke(context);
                return;
            }

            var oldAppBuilderServices = _appBuilder.ApplicationServices;

            try
            {
                // Can't remember why this is necessary to swap appBuilder.ApplicationServices here.. might be some mvc thing, need to rediscover.
                _logger.LogDebug("Setting AppBuilder Services to Tenant Container: {containerId} - {containerName}", tenantContainer.ContainerId, tenantContainer.ContainerName);
                _appBuilder.ApplicationServices = tenantContainer;
                var perRequestContainer = await requestContainerAccessor.TenantRequestContainer.Value;
              
                // Ensure per request container is disposed at end of request.
                context.Response.RegisterForDispose(perRequestContainer);
                
                // Replace request services with a nested version (for lifetime management - used to encpasulate a request).
                _logger.LogDebug("Setting Request Container: {containerId} - {containerName}", perRequestContainer.ContainerId, perRequestContainer.ContainerName);

                requestServicesSwapper.SwapRequestServices(perRequestContainer);
                //  var swapContextRequestServices = new RequestServicesSwapper(perRequestContainer);                
                // swapContextRequestServices.SwapRequestServices()
                await _next?.Invoke(context);
                 //await swapContextRequestServices.ExecuteWithinSwappedRequestContainer(_next, context);
                _logger.LogDebug("Restoring Request Container");
            }
            finally
            {
                _logger.LogDebug("Restoring AppBuilder Services");
                _appBuilder.ApplicationServices = oldAppBuilderServices;
            }
        }
    }
}
