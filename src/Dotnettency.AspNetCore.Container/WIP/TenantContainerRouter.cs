using System;
using System.Threading.Tasks;
using Dotnettency.AspNetCore.Container;
using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dotnettency.AspNetCore.Routing
{
  
    public class TenantContainerRouter<TTenant> : INamedRouter
        where TTenant : class
    {

        private readonly ILogger<TenantContainerRouter<TTenant>> _logger;
        private readonly IApplicationBuilder _rootAppBuilder;
        private readonly Action<IRouteBuilder> _configureChildRoutes;

        public TenantContainerRouter(string name, ILogger<TenantContainerRouter<TTenant>> logger, IApplicationBuilder rootAppBuilder, Action<IRouteBuilder> configureChildRoutes)
        {
            Name = name;
            _logger = logger;
            _rootAppBuilder = rootAppBuilder;
            _configureChildRoutes = configureChildRoutes;
            var routeBuilder = new RouteBuilder(rootAppBuilder, null);
            configureChildRoutes(routeBuilder);
            ChildRouter = routeBuilder.Build();
        }

        public string Name { get; set; }

        public IRouter ChildRouter { get; set; }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public async Task RouteAsync(RouteContext context)
        {

            //  context.han
            var tenantContainerAccessor = context.HttpContext.RequestServices.GetRequiredService<ITenantContainerAccessor<TTenant>>();
            var requestContainerAccessor = context.HttpContext.RequestServices.GetRequiredService<ITenantRequestContainerAccessor<TTenant>>();
            var swapper = context.HttpContext.RequestServices.GetRequiredService<RequestServicesSwapper<TTenant>>();

            _logger.LogDebug("Tenant Container Middleware - Start.");

            var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
            if (tenantContainer == null)
            {
                _logger.LogDebug("Tenant Container Middleware - No tenant container.");
                return;
            }

            context.RouteData.Routers.Add(this);
            context.RouteData.DataTokens.Add("TenantContainer", tenantContainer);
            context.RouteData.DataTokens.Add("RootAppBuilder", _rootAppBuilder);
            await ChildRouter.RouteAsync(context);

            // If we have any downstream route handler, wrap it with per request container before executing.
            if (context.Handler != null)
            {
                var wrapped = context.Handler;
                context.Handler = async (httpContext) =>
                {
                    var perRequestContainer = await requestContainerAccessor.TenantRequestContainer.Value;
                    context.HttpContext.Response.RegisterForDispose(perRequestContainer);
                    _logger.LogDebug("Setting Request Container: {containerId} - {containerName}", perRequestContainer.ContainerId, perRequestContainer.ContainerName);

                    swapper.SwapRequestServices(perRequestContainer);
                    await wrapped(context.HttpContext);

                };
            }

        }    
    }
}
