using Microsoft.AspNetCore.Http;

namespace Dotnettency
{
    public class HostnameAndPortTenantDistinguisherFactory<TTenant> : HttpContextTenantDistinguisherFactory<TTenant>
         where TTenant : class
    {

        private static int DefaultHttpPort = 80;
        private static int DefaultHttpsPort = 443;

        public HostnameAndPortTenantDistinguisherFactory(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        protected override TenantDistinguisher GetTenantDistinguisher(HttpContext context)
        {
            var value = GetHostNameWithPortNumber(context);
            var identity = new TenantDistinguisher(value);
            return identity;
        }

        public string GetHostNameWithPortNumber(HttpContext context)
        {
            var hostName = context.Request.Host.Value.ToLower();
            string hostNameWithoutPort = hostName;
            int port;

            if (hostName.Contains(":"))
            {
                return hostName;
            }
            else
            {
                if (context.Request.IsHttps)
                {
                    port = DefaultHttpsPort;
                }
                else
                {
                    port = DefaultHttpPort;
                }
            }

            string hostNameWithPort = $"{hostName}:{port}";
            return hostNameWithPort.ToLower();
        }

    }
}