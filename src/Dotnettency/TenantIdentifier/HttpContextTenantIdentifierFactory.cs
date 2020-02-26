using System.Threading.Tasks;

namespace Dotnettency
{
    public abstract class HttpContextTenantIdentifierFactory<TTenant> : ITenantIdentifierFactory<TTenant>
        where TTenant : class
    {
        protected IHttpContextProvider HttpContextAccessor { get; }

        protected const int DefaultHttpPort = 80;
        protected const int DefaultHttpsPort = 443;

        protected const string SchemeDelimiter = "://";

        protected HttpContextTenantIdentifierFactory(IHttpContextProvider httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public virtual Task<TenantIdentifier> IdentifyTenant()
        {
            TenantIdentifier identity = null;
            var context = HttpContextAccessor.GetCurrent();
            if (HttpContextAccessor != null)
            {
                identity = GetTenantIdentifier(context);
            }
            return Task.FromResult(identity);
        }

      
        protected abstract TenantIdentifier GetTenantIdentifier(HttpContextBase context);
    }
}
