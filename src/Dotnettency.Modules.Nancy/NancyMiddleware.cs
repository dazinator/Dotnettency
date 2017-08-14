using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dotnettency.Container;
using Dotnettency.Modules.Nancy;

namespace Dotnettency.Modules
{

    public class NancyMiddleware<TTenant>
        where TTenant : class
    {

        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly ILogger<NancyMiddleware<TTenant>> _logger;
        //   private readonly IModuleManager<TModule> _moduleManager;

        public NancyMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            ILogger<NancyMiddleware<TTenant>> logger
           )
        {
            _next = next;
            _rootApp = rootApp;
            _logger = logger;
            // _moduleManager = moduleManager;
        }



        public async Task Invoke(HttpContext context, ITenantNancyBootstrapperAccessor<TTenant> tenantNancyBootstrapper, ITenantContainerAccessor<TTenant> tenantContainerAccessor, ITenantRequestContainerAccessor<TTenant> tenantRequestContainerAccessor)
        {

            // get the nancy bootstrapper,
            // adjust its request container - give it the current request container to return.
            // get the nancy engine

            //  var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
            var tenantRequestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;
            var nancyBootstrapper = await tenantNancyBootstrapper.Bootstrapper.Value;

            if (tenantRequestContainer == null || nancyBootstrapper == null)
            {
                await _next.Invoke(context);
                return;
            }

            // swap out nancy request services.
            ITenantContainerAdaptor old = nancyBootstrapper.RequestContainerAdaptor;
            try
            {
                nancyBootstrapper.RequestContainerAdaptor = tenantRequestContainer.RequestContainer;
                var engine = nancyBootstrapper.GetEngine();
                var nancyHandler = new NancyHandler(engine);
                await nancyHandler.ProcessRequest(context, NancyPassThroughOptions.PassThroughWhenStatusCodesAre(global::Nancy.HttpStatusCode.NotFound), _next);
            }
            finally
            {
                nancyBootstrapper.RequestContainerAdaptor = old;
            }
        }
    }
}
