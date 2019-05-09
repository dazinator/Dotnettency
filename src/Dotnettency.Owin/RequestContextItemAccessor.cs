using System;

namespace Dotnettency
{
    //public class RequestContextItemAccessor<TItem> : IRequestContextItemAccessor<TItem>
    //    where TItem : class, IDisposable
    //{
    //    private readonly IHttpContextProvider _httpContextProvider;
    //    private readonly IServiceProvider _sp;
    //    private readonly Func<IServiceProvider, TItem> _factory;
    //    private readonly bool _disposeOnRequestCompletion;

    //    public RequestContextItemAccessor(IHttpContextProvider httpContextProvider, IServiceProvider sp, Func<IServiceProvider, TItem> factory, bool disposeOnRequestCompletion = true)
    //    {
    //        _httpContextProvider = httpContextProvider;
    //        _sp = sp;
    //        _factory = factory;
    //        _disposeOnRequestCompletion = disposeOnRequestCompletion;
    //    }

    //    public TItem GetCurrent()
    //    {
    //        // Gte current http context,
    //        // if no http context then return null;
    //        var context = _httpContextProvider.GetCurrent();
    //        if (context == null)
    //        {
    //            return null;
    //        }

    //        // get item if it exists,
    //        var item = context.GetItem<TItem>(nameof(TItem));
    //        if(item == null)
    //        {
    //            // consider locking
    //            item = _factory?.Invoke(_sp);
    //            if(item != null)
    //            {
    //                context.SetItem(nameof(TItem), item, _disposeOnRequestCompletion);
    //            }
    //        }
    //        return item;
    //        //if(item == null)
    //        //{

    //        //}
    //        //// if it doesn't exist, call factory to create it and store it in current request context, to be disposed of at end of request.
    //        //throw new NotImplementedException();
    //    }
    //}
}
