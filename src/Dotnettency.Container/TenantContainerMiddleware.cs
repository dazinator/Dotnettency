using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace WebExperiment
{
    public class TenantContainerMiddleware<TTenant>
         where TTenant : class
    {

        private readonly RequestDelegate next;
        private readonly ILogger log;

        public TenantContainerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)

        {
            this.next = next;
            this.log = loggerFactory.CreateLogger<TenantContainerMiddleware<TTenant>>();

        }



        public async Task Invoke(HttpContext context, MultitenancyProvider<TTenant> multitenancyProvider)
        {
            log.LogDebug("Using multitenancy provider {multitenancyProvidertype}.", multitenancyProvider.GetType().Name);
            var currentTenantContainer = await multitenancyProvider.CurrentTenantContainer.LazyResult.Value;


            // context.Features.Set<ITenantFeature<TTenant>>(feature);

            //var tenant = await feature.Tenant;
            //context.SetTenant(tenant);

            //todo: need to use a different container implementation that supports nested containers
            // sp that we can nest the tenant container onto the application container.
            using (var scope = currentTenantContainer.CreateNestedContainer())
            {
                var oldRequestServices = context.RequestServices;
                context.RequestServices = scope.ServiceProvider.Value;
                await next.Invoke(context);
                context.RequestServices = oldRequestServices;
            }


        }

    }


}
