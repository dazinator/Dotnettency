using System;

namespace Dotnettency.Container
{
    public class RequestServicesSwapper<TTenant> : IDisposable
        where TTenant : class
    {
        private Action _onDispose;

        //  private string _key;
       // private readonly ITenantRequestContainerAccessor<TTenant> _requestContainerAccessor;
        private readonly IHttpContextProvider _httpContextProvider;

        public RequestServicesSwapper(
            IHttpContextProvider httpContextProvider)
        {
           // _requestContainerAccessor = requestContainerAccessor;
            _httpContextProvider = httpContextProvider;
        }

        public void SwapRequestServices(IServiceProvider serviceProvider)
        {
            // var perRequestContainer = await _requestContainerAccessor.TenantRequestContainer.Value;
            var context = _httpContextProvider.GetCurrent();

            var old = context.GetRequestServices();
            context.SetRequestServices(serviceProvider);

            _onDispose = () =>
                {
                    context.SetRequestServices(old);
                };

        }

        public void Dispose()
        {
            _onDispose?.Invoke();
            _onDispose = null;
        }
    }
}
