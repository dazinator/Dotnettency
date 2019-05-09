using System;
using System.Collections.Generic;

namespace Dotnettency
{

    public interface IHttpContextProvider
    {
        HttpContextBase GetCurrent();
    }

    public abstract class HttpContextBase
    {
        protected const int DefaultHttpPort = 80;
        protected const int DefaultHttpsPort = 443;

        public abstract RequestBase Request { get; }
      //  public virtual Dictionary<string, object> Items { get; }

        public abstract void SetItem(string key, object item);

        public abstract void SetItem(string key, IDisposable item, bool disposeOnRequestCompletion = true);
        public abstract TItem GetItem<TItem>(string key)
            where TItem : class;
    }

    public abstract class RequestBase
    {
        public abstract Uri GetUri();

    }
}
