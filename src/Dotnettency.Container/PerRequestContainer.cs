using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.Container
{
    public class PerRequestContainer : IDisposable
    {
        private Action _onDispose;

        public PerRequestContainer(ITenantContainerAdaptor requestContainer)
        {
            RequestContainer = requestContainer;
        }

        public ITenantContainerAdaptor RequestContainer { get; private set; }
        
        public async Task ExecuteWithinSwappedRequestContainer(RequestDelegate request, HttpContext context)
        {
            if (context.Items.ContainsKey(nameof(PerRequestContainer)))
            {
                await request.Invoke(context);
                return;
            }

            context.Items[nameof(PerRequestContainer)] = this;
            var oldServiceProvider = context.RequestServices;

            try
            {
                _onDispose = () =>
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
            }
        }

        public void Dispose()
        {
            _onDispose?.Invoke();
        }
    }
}
