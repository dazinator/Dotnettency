using System;
using System.Web;

namespace Dotnettency.SystemWeb
{
    public class HttpContextProvider : IHttpContextProvider
    {
        protected HttpContextProvider()
        {
        }

        public HttpContextBase GetCurrent()
        {
            var httpContext =  HttpContext.Current;            
            var context = new HttpContextWrapper(httpContext);
            return context;
        }
    }
}
