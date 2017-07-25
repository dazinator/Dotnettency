using Microsoft.AspNetCore.Http;

namespace Dotnettency
{
    public class HostnameAndPortTenantDistinguisherFactory<TTenant> : HttpContextTenantDistinguisherFactory<TTenant>
         where TTenant : class
    {     

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
            var host = context.Request.Host;
            var port = host.Port.HasValue ? host.Port.Value : (context.Request.IsHttps ? DefaultHttpsPort : DefaultHttpPort);

            //   var hostName = context.Request.Host.Value.ToLower();
            string identfier = $"{host.Host}:{port}";
            return identfier.ToLowerInvariant();

        }
    }
}