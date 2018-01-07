using System.Threading.Tasks;
using Dotnettency.AspNetCore.MiddlewarePipeline;
using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dotnettency.AspNetCore.Routing
{

    public class TenantMiddlewarePipelineRouter<TTenant> : INamedRouter
        where TTenant : class
    {

        private readonly ILogger<TenantMiddlewarePipelineRouter<TTenant>> _logger;
        private readonly IApplicationBuilder _rootAppBuilder;
        // private readonly ITenantPipelineAccessor<TTenant> _pipelineAccessor;
        private readonly ITenantMiddlewarePipelineFactory<TTenant> _pipelineFactory;

        public TenantMiddlewarePipelineRouter(string name,
            ILogger<TenantMiddlewarePipelineRouter<TTenant>> logger,
            IApplicationBuilder rootAppBuilder,
            ITenantMiddlewarePipelineFactory<TTenant> pipelineFactory)
        {
            Name = name;
            _logger = logger;
            _rootAppBuilder = rootAppBuilder;
            _pipelineFactory = pipelineFactory;

        }

        public string Name { get; set; }

        // public IRouter ChildRouter { get; set; }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
            // throw new System.NotImplementedException();
            // return new VirtualPathData(this,"")
        }

        public async Task RouteAsync(RouteContext context)
        {

            //  context.han
            var sp = context.HttpContext.RequestServices;
            var tenantContainerAccessor = sp.GetRequiredService<ITenantContainerAccessor<TTenant>>();
            var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;

          //  Microsoft.Extensions.DependencyInjection.ActivatorUtilities.GetServiceOrCreateInstance<>
            var tenantPipelineAccessor = tenantContainer.GetRequiredService<ITenantPipelineAccessor<TTenant>>();



            _logger.LogDebug("Tenant Pipeline Router - Getting Tenant Pipeline.");
            var tenantPipeline = await tenantPipelineAccessor.TenantPipeline(_rootAppBuilder, tenantContainer, null, _pipelineFactory).Value;

            if (tenantPipeline != null)
            {
                _logger.LogDebug("Tenant Pipeline Router - Executing Pipeline.");
                context.RouteData.Routers.Add(this);
                //context.RouteData.DataTokens.Add("TenanMiddlewarePipeline", tenantPipeline);
                context.Handler = tenantPipeline;
            }
            else
            {
                _logger.LogDebug("No Tenant Middleware Pipeline to execute.");
                return;
            }

        }
    }
}
