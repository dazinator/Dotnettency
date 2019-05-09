using System.Threading.Tasks;
using DavidLievrouw.OwinRequestScopeContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Owin
{
    //public class DotnettencyOwinMiddleware : OwinMiddleware
    //{
    //    readonly Func<IDictionary<string, object>, Task> _next;

    //    public DotnettencyOwinMiddleware(
    //      Func<IDictionary<string, object>, Task> next)
    //    {
    //        if (next == null) throw new ArgumentNullException(paramName: nameof(next));
    //        _next = next;
    //    }

    //    public async Task Invoke(IDictionary<string, object> environment)
    //    {
    //        // This middleware is triggered before the OwinRequestScopeContext middleware
    //        // There should be no instance present

    //        var requestScopeContext = OwinRequestScopeContext.Current;
    //        var requestContext = environment["System.Web.HttpContextBase"] as HttpContextBase;
    //        if (requestScopeContext != null) throw new InvalidOperationException(message: $"There should not be an {requestScopeContext.GetType().Name} object available here.");

    //        Debug.WriteLine($"The {typeof(OwinRequestScopeContext).Name} instance needs to be created.");

    //        await _next.Invoke(environment).ConfigureAwait(false);
    //    }
    //}

    public class RequestScopeItemMiddleware<TItem>
        where TItem : IDisposable
    {

        readonly Func<IDictionary<string, object>, Task> _next;
        private readonly Func<TItem> _factory;

        public RequestScopeItemMiddleware(AppFunc next, Func<TItem> serviceScopeFactory)
        {
            _next = next;
            _factory = serviceScopeFactory;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var requestScopeContext = OwinRequestScopeContext.Current;
            //  var requestContext = environment["System.Web.HttpContextBase"] as HttpContextBase;
            if (requestScopeContext == null) throw new InvalidOperationException(message: $"There should be a {requestScopeContext.GetType().Name} object available, have you registered UseRequestScopeContext() ?");

            var item = _factory();
            requestScopeContext.Items.Add(nameof(TItem), item, true);
            await _next?.Invoke(environment);

        }
    }


}