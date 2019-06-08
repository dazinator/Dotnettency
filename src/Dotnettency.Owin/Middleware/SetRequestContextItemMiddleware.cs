using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Dotnettency.Owin
{

    /// <summary>
    /// Middleware that creates an instance of <see cref="TItem"/> and stores it in request items, optionally disposing it once request is processed.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class SetRequestContextItemMiddleware<TItem>
        where TItem : IDisposable
    {

        readonly Func<IDictionary<string, object>, Task> _next;
        private readonly Owin.SetRequestContextItemMiddlewareOptions<TItem> _options;

        public SetRequestContextItemMiddleware(AppFunc next, SetRequestContextItemMiddlewareOptions<TItem> options)
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var item = _options.Factory.Invoke();
            _options.HttpContextProvider.GetCurrent().SetItem(typeof(TItem).Name, item, _options.DisposeAtEndOfRequest);
            _options.OnInstanceCreated?.Invoke(item);
            await _next?.Invoke(environment);
        }
    }
}