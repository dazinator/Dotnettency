using Microsoft.AspNetCore.Http;

namespace Dotnettency
{
    public class RequestUriTenantDistinguisherFactory<TTenant> : HttpContextTenantDistinguisherFactory<TTenant>
         where TTenant : class
    {

        public RequestUriTenantDistinguisherFactory(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        protected override TenantDistinguisher GetTenantDistinguisher(HttpContext context)
        {
            var uri = context.Request.GetUri();
            var identity = new TenantDistinguisher(uri);
            return identity;
        }


    }
}