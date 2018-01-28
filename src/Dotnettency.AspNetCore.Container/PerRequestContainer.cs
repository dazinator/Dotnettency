using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Dotnettency.Container;

namespace Dotnettency.AspNetCore.Container
{
    public class PerRequestContainer : IDisposable
    {
        private Action _onDispose;

        private string _key;

        public PerRequestContainer(ITenantContainerAdaptor requestContainer)
        {
            RequestContainer = requestContainer;
            _key = nameof(PerRequestContainer) + RequestContainer.ContainerId;
        }

        public ITenantContainerAdaptor RequestContainer { get; private set; }

        public async Task ExecuteWithinSwappedRequestContainer(RequestDelegate request, HttpContext context)
        {          
            var oldServiceProvider = context.RequestServices;

            try
            {
                if (!context.Items.ContainsKey(_key))
                {
                    context.Items[_key] = this;

                    _onDispose = () =>
                    {
                        context.Items.Remove(_key);
                        RequestContainer.Dispose();
                    };
                }

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
