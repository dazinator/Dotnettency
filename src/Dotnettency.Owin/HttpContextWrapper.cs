using DavidLievrouw.OwinRequestScopeContext;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Owin
{
    public class HttpContextWrapper : HttpContextBase
    {
        private readonly IOwinRequestScopeContext _context;
        private static readonly string requestServicesKey = "Request" + typeof(IServiceProvider).Name;

        public HttpContextWrapper(IOwinRequestScopeContext context)
        {
            Request = new HttpRequestWrapper(context.OwinEnvironment);
            _context = context;
        }
        public override RequestBase Request { get; }

        public static string RequestServicesKey => requestServicesKey;

        public override bool ContainsKey(string key)
        {
            return _context.Items.ContainsKey(key);
        }

        public override TItem GetItem<TItem>(string key)
        {
            _context.Items.TryGetValue(key, out object val);
            return val as TItem;
        }        

        public override IServiceProvider GetRequestServices()
        {
            _context.Items.TryGetValue(RequestServicesKey, out object sp);
            return sp as IServiceProvider;
        }

        public override void SetRequestServices(IServiceProvider requestServices)
        {
            SetItem(RequestServicesKey, requestServices);
        }

        public override void SetItem(string key, object item)
        {
            _context.Items[key] = item;
        }

        public override void SetItem(string key, IDisposable item, bool disposeOnRequestCompletion = true)
        {
            _context.Items.Add(key, item, true);
        }
    }

}