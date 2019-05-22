using System;

namespace Dotnettency.Owin
{
    public class SetRequestContextItemMiddlewareOptions<TItem>
        where TItem : IDisposable
    {

        public Func<TItem> Factory { get; set; }
        public IHttpContextProvider HttpContextProvider { get; set; }

        public bool DisposeAtEndOfRequest { get; set; }
        public Action<TItem> OnInstanceCreated { get; internal set; }
    }
}