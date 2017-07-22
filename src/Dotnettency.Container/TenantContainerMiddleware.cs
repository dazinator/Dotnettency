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

            if (tenantShell != null)
            {
                var tenant = tenantShell?.Tenant;

                // Only create the factory if not already created.
                if (_containerFactory == null)
                {
                    _containerFactory = new Lazy<Task<ITenantContainerAdaptor>>(() =>
                    {
                        return _factory.Get(tenant);
                    });
                }

                var lazy = tenantShell.GetOrAddContainer<TTenant>(_containerFactory);
                var currentTenantContainer = await lazy.Value;


                using (var scope = currentTenantContainer.CreateNestedContainer())
                {
                    var oldRequestServices = context.RequestServices;
                    context.RequestServices = scope.ServiceProvider.Value;
                    await _next.Invoke(context);
                    context.RequestServices = oldRequestServices;
                }
            };








        }

    }


}
