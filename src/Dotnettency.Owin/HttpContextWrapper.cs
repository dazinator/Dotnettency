using DavidLievrouw.OwinRequestScopeContext;
using System;

namespace Dotnettency.Owin
{
    public class HttpContextWrapper : HttpContextBase
    {
        private readonly IOwinRequestScopeContext _context;

        public HttpContextWrapper(IOwinRequestScopeContext context)
        {
            Request = new HttpRequestWrapper(context.OwinEnvironment);
            _context = context;
        }
        public override RequestBase Request { get; }

        public override TItem GetItem<TItem>(string key)            
        {
            _context.Items.TryGetValue(key, out object val);
            return val as TItem;
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