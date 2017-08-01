using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dotnettency.Container;

namespace Dotnettency.Modules
{

    public class ModulesMiddleware<TTenant, TModule>
        where TTenant : class
        where TModule : IModule
    {

        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly ILogger<ModulesMiddleware<TTenant, TModule>> _logger;
        private readonly IModuleManager<TModule> _moduleManager;

        public ModulesMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            ILogger<ModulesMiddleware<TTenant, TModule>> logger,
             IModuleManager<TModule> moduleManager
           )
        {
            _next = next;
            _rootApp = rootApp;
            _logger = logger;
            _moduleManager = moduleManager;
        }


        public async Task Invoke(HttpContext context, ITenantContainerAccessor<TTenant> tenantContainerAccessor)
        {

            // need to ensure all modules are initialised.
            await _moduleManager.EnsureModulesStarted(() =>
            {
                return tenantContainerAccessor.TenantContainer.Value;
            }, _rootApp);

            var router = _moduleManager.GetModulesRouter();
            var routeContext = new Modules.ModulesRouteContext<TModule>(context);

            // context.GetRouteData().Routers.Add(router);
            await router.RouteAsync(routeContext);

            if (routeContext.Handler == null)
            {
                _logger.LogInformation("Request did not match routes for any modules..");
                await _next.Invoke(context);
            }
            else
            {
                // we can also store the modules container.
                //  context.Features[typeof(IRoutingFeature)] = new RoutingFeature()
                var routedModule = routeContext.ModuleShell;
                _logger.LogInformation("Request matched module {0}", routedModule.Module.GetType().Name);


                // Replace request services with a nested version of the routed modules container.
                using (var scope = routedModule.Container.CreateNestedContainer())
                {

                    _logger.LogDebug("Setting Request: {containerId} - {containerName}", scope.ContainerId, scope.ContainerName);
                    var oldRequestServices = context.RequestServices;
                    context.RequestServices = scope.GetServiceProvider();
                    await routeContext.Handler(routeContext.HttpContext);
                    _logger.LogDebug("Restoring Request Container");
                    context.RequestServices = oldRequestServices;
                }
            }
        }
    }
}
