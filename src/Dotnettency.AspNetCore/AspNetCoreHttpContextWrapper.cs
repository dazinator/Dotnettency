using Microsoft.AspNetCore.Http;
using System;

namespace Dotnettency.AspNetCore
{
    public class AspNetCoreHttpContextWrapper : HttpContextBase
    {
        private readonly HttpContext _context;

        public AspNetCoreHttpContextWrapper(HttpContext context)
        {
            this._context = context;
            this.Request = new AspNetCoreRequestWrapper(context.Request);
        }

        public override RequestBase Request { get; }

        public override TItem GetItem<TItem>(string key)
        {
            _context.Items.TryGetValue(key, out object item);
            return item as TItem;
        }

        public override void SetItem(string key, object item)
        {
            _context.Items[key] = item;
        }

        public override void SetItem(string key, IDisposable item, bool disposeOnRequestCompletion = true)
        {
            _context.Items[key] = item;
            if(disposeOnRequestCompletion)
            {
                _context.Response.RegisterForDispose(item);
            }                    
        }
    }
}
