using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dotnettency
{
    public abstract class HttpContextTenantDistinguisherFactory<TTenant> : ITenantDistinguisherFactory<TTenant>
        where TTenant : class
    {
        private readonly IHttpContextAccessor _httpContextAccesor;

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


        protected IHttpContextAccessor HttpContextAccessor { get { return _httpContextAccesor; } }
    }

}
