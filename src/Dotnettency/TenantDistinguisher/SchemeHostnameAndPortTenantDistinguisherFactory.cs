using Microsoft.AspNetCore.Http;

namespace Dotnettency
{

    public class SchemeHostnameAndPortTenantDistinguisherFactory<TTenant> : HttpContextTenantDistinguisherFactory<TTenant>
         where TTenant : class
    {   

        public SchemeHostnameAndPortTenantDistinguisherFactory(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        protected override TenantDistinguisher GetTenantDistinguisher(HttpContext context)
        {
            var value = GetSchemeAndHostNameAndPortNumber(context);
            var identity = new TenantDistinguisher(value);
            return identity;
        }

        public string GetSchemeAndHostNameAndPortNumber(HttpContext context)
        {

            var host = context.Request.Host;
            var port = host.Port.HasValue ? host.Port.Value : (context.Request.IsHttps ? DefaultHttpsPort : DefaultHttpPort);

            //   var hostName = context.Request.Host.Value.ToLower();
            string identfier = $"{context.Request.Scheme}://{host.Host}:{port}";
            return identfier.ToLowerInvariant();

        }
    }
}