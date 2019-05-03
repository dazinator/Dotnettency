using System.Threading.Tasks;
using DavidLievrouw.OwinRequestScopeContext;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

}