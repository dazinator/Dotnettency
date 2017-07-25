using Microsoft.AspNetCore.Http;

namespace Dotnettency
{
    public class HostnameTenantDistinguisherFactory<TTenant> : HttpContextTenantDistinguisherFactory<TTenant>
         where TTenant : class
    {
        public HostnameTenantDistinguisherFactory(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        protected override TenantDistinguisher GetTenantDistinguisher(HttpContext context)
        {
            var host = context.Request.Host.Host;
            var identity = new TenantDistinguisher(host);
            return identity;
        }

    }
}