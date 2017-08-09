using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.Container
{
    public class PerRequestContainer : IDisposable
    {

        private bool _hasSwapped = false;

        public PerRequestContainer(ITenantContainerAdaptor requestContainer, HttpContext httpContext)
        {
            RequestContainer = requestContainer;
            HttpContext = httpContext;
            HttpContext.Items[nameof(PerRequestContainer)] = requestContainer;

            //var sp = requestContainer.GetServiceProvider();

            //var sp = requestContainer.GetServiceProvider();
            //var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            //var scope = scopeFactory.CreateScope();



            //var scope = RequestContainer.GetServiceProvider();

            // Replace request services with a nested version (for lifetime management - used to encpasulate a request).
            //using (var scope = tenantContainer.CreateNestedContainer())
            //{
            //_logger.LogDebug("Setting Request: {containerId} - {containerName}", scope.ContainerId, scope.ContainerName);
            //var oldRequestServices = context.RequestServices;
            //context.RequestServices = scope.GetServiceProvider();
            ////  await _next.Invoke(context); // module middleware should be next - which will replace again with module specific container (nested).
            //// _log.LogDebug("Restoring Request Container");
            //context.RequestServices = oldRequestServices;
            //}


            //var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            // var scope = new StructureMapServiceScope()
            // Scope = scopeFactory.CreateScope();
        }

        public HttpContext HttpContext { get; }

        public ITenantContainerAdaptor RequestContainer { get; }

        public async Task ExecuteWithinSwappedRequestContainer(Task task)
        {
            if(!_hasSwapped)
            {              
                IServiceProvider oldServiceProvider = HttpContext.RequestServices;
                try
                {
                    HttpContext.RequestServices = RequestContainer.GetServiceProvider();
                    await task;
                }
                finally
                {
                    HttpContext.RequestServices = oldServiceProvider;
                    // throw;
                }
                _hasSwapped = true;
            }
           
        }
        // public IServiceScope Scope { get; }

        public void Dispose()
        {
            HttpContext.Items.Remove(nameof(PerRequestContainer));
            RequestContainer.Dispose();
            // Scope.Dispose();
        }
    }
}