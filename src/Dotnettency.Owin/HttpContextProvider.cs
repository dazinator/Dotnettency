using DavidLievrouw.OwinRequestScopeContext;

namespace Dotnettency.Owin
{
    public class HttpContextProvider : IHttpContextProvider
    {
        public HttpContextProvider()
        {
        }

        public HttpContextBase GetCurrent()
        {
            var requestScopeContext = OwinRequestScopeContext.Current;
            var context = new HttpContextWrapper(requestScopeContext.OwinEnvironment);
            return context;
        }
    }
}