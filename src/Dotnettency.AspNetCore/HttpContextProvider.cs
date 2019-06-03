using Microsoft.AspNetCore.Http;

namespace Dotnettency.AspNetCore
{
    public class HttpContextProvider : IHttpContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccesor;
        public HttpContextProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccesor = httpContextAccessor;
        }

        public HttpContextBase GetCurrent()
        {
            var httpContext = _httpContextAccesor.HttpContext;
            if(httpContext == null)
            {
                return null;
            }
            var context = new AspNetCoreHttpContextWrapper(httpContext);
            return context;
        }
    }
}
