using Microsoft.Extensions.Logging;

namespace Dotnettency.Owin
{
    public class TenantContainerMiddlewareOptions
    {
        public IHttpContextProvider HttpContextProvider { get; set; }

       public ILoggerFactory LoggerFactory { get; set; }

        public bool DisposeAtEndOfRequest { get; set; }
    }
}