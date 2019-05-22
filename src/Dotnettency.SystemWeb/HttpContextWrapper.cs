using System;
using System.Web;

namespace Dotnettency.SystemWeb
{
    public class HttpContextWrapper : HttpContextBase
    {
        private readonly HttpContext _context;
        private static readonly string RequestServicesKey = "Request" + typeof(IServiceProvider).Name;

        public HttpContextWrapper(HttpContext context)
        {
            this._context = context;
            this.Request = new Dotnettency.SystemWeb.HttpRequestWrapper(context.Request);
        }

        public override RequestBase Request { get; }

        public override TItem GetItem<TItem>(string key)
        {
            if (_context.Items.Contains(key))
            {
                return _context.Items[key] as TItem;
            }

            return null;
        }

        public override void SetItem(string key, object item)
        {
            _context.Items[key] = item;
        }

        public override void SetItem(string key, IDisposable item, bool disposeOnRequestCompletion = true)
        {
            SetItem(key, item);
            if (disposeOnRequestCompletion)
            {
                _context.DisposeOnPipelineCompleted(item);
            }
        }

        public override IServiceProvider GetRequestServices()
        {
            var sp = GetItem<IServiceProvider>(RequestServicesKey);
            return sp;
        }

        public override void SetRequestServices(IServiceProvider requestServices)
        {
            SetItem(RequestServicesKey, requestServices);
        }

        public override bool ContainsKey(string key)
        {
            return _context.Items.Contains(key);
        }
    }
}
