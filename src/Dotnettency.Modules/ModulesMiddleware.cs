using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{
    public class ModulesMiddleware<TTenant, TModule>
        where TTenant : class
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly ILogger<ModulesMiddleware<TTenant, TModule>> _logger;
        private readonly IModuleManager<TModule> _moduleManager;

        public ModulesMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            ILogger<ModulesMiddleware<TTenant, TModule>> logger,
            IModuleManager<TModule> moduleManager)
        {
            _next = next;
            _rootApp = rootApp;
            _logger = logger;
            _moduleManager = moduleManager;
        }

        public async Task Invoke(HttpContext context, ITenantContainerAccessor<TTenant> tenantContainerAccessor)
        {
            // need to ensure all modules are initialised.
            await _moduleManager.EnsureStarted(() =>
            {
                return tenantContainerAccessor.TenantContainer.Value;
            }, _rootApp);

            var router = _moduleManager.GetModulesRouter();
            var routeContext = new ModulesRouteContext<TModule>(context);
            routeContext.RouteData.Routers.Add(router);

            await router.RouteAsync(routeContext);

            if (routeContext.Handler == null)
            {
                _logger.LogDebug("Request did not match routes for any modules..");
                await _next.Invoke(context);
                return;
            }

            var routedModule = routeContext.ModuleShell;

            routeContext.HttpContext.Features[typeof(IRoutingFeature)] = new RoutingFeature()
            {
                RouteData = routeContext.RouteData,
            };

            _logger.LogDebug("Request matched module {0}", routedModule.Module.GetType().Name);

            // Replace request services with a nested version of the routed modules container.
            using (var scope = routedModule.Container.CreateNestedContainer())
            {
                _logger.LogDebug("Setting Request: {containerId} - {containerName}", scope.ContainerId, scope.ContainerName);
                var oldRequestServices = context.RequestServices;
                context.RequestServices = scope;
                await routeContext.Handler(routeContext.HttpContext);
                _logger.LogDebug("Restoring Request Container");
                context.RequestServices = oldRequestServices;
            }
        }
    }
}
