using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Dotnettency.Container;
using Microsoft.AspNetCore.Routing;
using Dotnettency.Modules.Nancy;
using System;

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

            var tenantContainer = await tenantContainerAccessor.TenantContainer.Value;
            var tenantRequestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;
            var nancyBootstrapper = await tenantNancyBootstrapper.Bootstrapper.Value;

            if (tenantContainer == null || tenantRequestContainer == null || nancyBootstrapper == null)
            {
                await _next.Invoke(context);
                return;
            }

            // swap out nancy request services.
            ITenantContainerAdaptor old = nancyBootstrapper.RequestContainerAdaptor;
            try
            {
                nancyBootstrapper.RequestContainerAdaptor = tenantRequestContainer.RequestContainer;

                // proceed with nancy request.
                throw new NotImplementedException();
                await _next.Invoke(context);
            }
            finally
            {
                nancyBootstrapper.RequestContainerAdaptor = old;
            }
        }
    }
}
