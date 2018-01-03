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

        public TenantMiddlewarePipelineRouter(string name, ILogger<TenantMiddlewarePipelineRouter<TTenant>> logger, IApplicationBuilder rootAppBuilder)
        {
            Name = name;
            _logger = logger;
            _rootAppBuilder = rootAppBuilder;
            //   ChildRouter = new RouteCollection();
            //  _tenantDistinguisherFactory = tenantDistinguisherFactory;
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
            var tenantContainerAccessor = context.HttpContext.RequestServices.GetRequiredService<ITenantContainerAccessor<TTenant>>();
            var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
            var tenantPipelineAccessor = context.HttpContext.RequestServices.GetRequiredService<ITenantPipelineAccessor<TTenant>>();

            _logger.LogDebug("Tenant Pipeline Router - Getting Tenant Pipeline.");
            var tenantPipeline = await tenantPipelineAccessor.TenantPipeline(_rootAppBuilder, tenantContainer, null).Value;

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
