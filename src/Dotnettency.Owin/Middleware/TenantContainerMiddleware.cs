using System.Threading.Tasks;
using System.Collections.Generic;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
using Microsoft.Extensions.Logging;
using Dotnettency.Middleware;
using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Owin
{
    public class TenantContainerMiddleware<TTenant>
      where TTenant : class
    {
        private readonly AppFunc _next;
        private readonly TenantContainerMiddlewareOptions _options;
        // private readonly ILogger<TenantContainerMiddleware<TTenant>> _logger;
        // private readonly AppBuilderAdaptorBase _appBuilder;

        public TenantContainerMiddleware(
            AppFunc next,
            TenantContainerMiddlewareOptions options)
        {
            _next = next;
            this._options = options;
            //  _logger = logger;
            // _appBuilder = appBuilder;
        }

        public async Task Invoke(IDictionary<string, object> context)
        {
            // below has go be scoped to request..
            //  var owin = new OwinEnvironment()
            var provider = _options.HttpContextProvider;
            var requestServices = provider.GetCurrent().GetRequestServices();

            var tenantContainerAccessor = requestServices.GetService<ITenantContainerAccessor<TTenant>>();
            var tenantRequestContainerAccessor = requestServices.GetService<ITenantRequestContainerAccessor<TTenant>>();
            var requestServicesSwapper = requestServices.GetService<RequestServicesSwapper<TTenant>>();

            //  _logger.LogDebug("Tenant Container Middleware - Start.");

            var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
            if (tenantContainer == null)
            {
                // _logger.LogDebug("Tenant Container Middleware - No tenant container.");
                await _next.Invoke(context);
                return;
            }

            //   var oldAppBuilderServices = _appBuilder.ApplicationServices;

            try
            {
                //  _logger.LogDebug("Setting AppBuilder Services to Tenant Container: {containerId} - {containerName}", tenantContainer.ContainerId, tenantContainer.ContainerName);
                //  _appBuilder.ApplicationServices = tenantContainer;
                var perRequestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;
                // Ensure this per request container is disposed at end of request.
                var httpContext = _options.HttpContextProvider.GetCurrent();
                httpContext.SetItem(typeof(ITenantContainerAdaptor).Name, perRequestContainer, true);
                requestServicesSwapper.SwapRequestServices(perRequestContainer);

                // Replace request services with a nested version (for lifetime management - used to encpasulate a request).
                //   _logger.LogDebug("Setting Request Container: {containerId} - {containerName}", perRequestContainer.RequestContainer.ContainerId, perRequestContainer.RequestContainer.ContainerName);
                await _next?.Invoke(context);
                //  _logger.LogDebug("Restoring Request Container");
            }
            finally
            {
                //  _logger.LogDebug("Restoring AppBuilder Services");
                //  _appBuilder.ApplicationServices = oldAppBuilderServices;
            }
        }
    }
}