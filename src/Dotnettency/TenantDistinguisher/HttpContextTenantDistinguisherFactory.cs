using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dotnettency
{
    public abstract class HttpContextTenantDistinguisherFactory<TTenant> : ITenantDistinguisherFactory<TTenant>
        where TTenant : class
    {
        private readonly IHttpContextAccessor _httpContextAccesor;
        protected IHttpContextAccessor HttpContextAccessor => _httpContextAccesor;

        protected const int DefaultHttpPort = 80;
        protected const int DefaultHttpsPort = 443;

        public HttpContextTenantDistinguisherFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccesor = httpContextAccessor;
        }

        public virtual Task<TenantDistinguisher> IdentifyContext()
        {
            TenantDistinguisher identity = null;
            if (HttpContextAccessor.HttpContext != null)
            {
                identity = GetTenantDistinguisher(HttpContextAccessor.HttpContext);
            }
            return Task.FromResult(identity);
        }

        protected abstract TenantDistinguisher GetTenantDistinguisher(HttpContext context);
    }
}
