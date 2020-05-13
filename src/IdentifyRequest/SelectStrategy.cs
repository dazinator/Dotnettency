using Microsoft.AspNetCore.Http;

namespace IdentifyRequest
{
    public static class SelectValueStrategy
    {
        public static SelectValue HostNoPort(this ValueSelectorStrategies options)
        {
            return new SelectValue(GetHostNoPort);
        }

        private static string GetHostNoPort(HttpContext httpContext)
        {
            // authorityUriBuilder.Host           
            return httpContext?.Request?.GetUri()?.Host;
        }       
    }

}
