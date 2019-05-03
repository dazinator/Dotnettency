using Microsoft.AspNetCore.Http;

namespace Dotnettency.AspNetCore
{
    public class HttpContextProvider : IHttpContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAccesor;
        protected HttpContextProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccesor = httpContextAccessor;
        }

        public HttpContextBase GetCurrent()
        {
            var httpContext = _httpContextAccesor.HttpContext;
            var context = new AspNetCoreHttpContextWrapper(httpContext);
            return context;
        }
    }
}
