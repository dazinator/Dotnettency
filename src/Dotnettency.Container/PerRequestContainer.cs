using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.Container
{
    public class PerRequestContainer : IDisposable
    {

        // private bool _hasSwapped = false;

        public PerRequestContainer(ITenantContainerAdaptor requestContainer)
        {
            RequestContainer = requestContainer;


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

        // public HttpContext HttpContext { get; }

        public ITenantContainerAdaptor RequestContainer { get; }

        private Action OnDispose { get; set; }

        public async Task ExecuteWithinSwappedRequestContainer(RequestDelegate request, HttpContext context)
        {
            if (!context.Items.ContainsKey(nameof(PerRequestContainer)))
            {
                context.Items[nameof(PerRequestContainer)] = this;
                IServiceProvider oldServiceProvider = context.RequestServices;
                try
                {
                    OnDispose = () =>
                    {
                        context.Items.Remove(nameof(PerRequestContainer));
                        RequestContainer.Dispose();
                    };
                    context.RequestServices = RequestContainer;
                    await request.Invoke(context);
                }
                finally
                {
                    context.RequestServices = oldServiceProvider;
                    // throw;
                }
            }

        }
        // public IServiceScope Scope { get; }

        public void Dispose()
        {
            OnDispose();
            // Scope.Dispose();
        }
    }
}