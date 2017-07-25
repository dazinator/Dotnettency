using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dotnettency.Container
{
    public class TenantContainerMiddleware<TTenant>
         where TTenant : class
    {

        private readonly RequestDelegate _next;
        private readonly ILogger _log;
        private readonly ITenantContainerFactory<TTenant> _factory;

        private Lazy<Task<ITenantContainerAdaptor>> _containerFactory;

        public TenantContainerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory,
             ITenantContainerFactory<TTenant> factory)
        {
            _next = next;
            _log = loggerFactory.CreateLogger<TenantContainerMiddleware<TTenant>>();
            _factory = factory;
        }

        public async Task Invoke(HttpContext context, ITenantShellAccessor<TTenant> tenantShellAccessor)
        {
            //  log.LogDebug("Using multitenancy provider {multitenancyProvidertype}.", tenantAccessor.GetType().Name);


            var tenantShell = await tenantShellAccessor.CurrentTenantShell.Value;
            if (tenantShell == null)
            {
                await _next.Invoke(context);
                return;
            }


            var tenant = tenantShell?.Tenant;
            var lazy = tenantShell.GetOrAddContainer<TTenant>(()=>_factory.Get(tenant));           
            var currentTenantContainer = await lazy.Value;

            using (var scope = currentTenantContainer.CreateNestedContainer())
            {

                _log.LogDebug("Setting Request: {containerId} - {containerName}", scope.ContainerId, scope.ContainerName);
                var oldRequestServices = context.RequestServices;
                context.RequestServices = scope.ServiceProvider.Value;
                await _next.Invoke(context);
                _log.LogDebug("Restoring Request Container");
                context.RequestServices = oldRequestServices;
            }

        }
    }
}