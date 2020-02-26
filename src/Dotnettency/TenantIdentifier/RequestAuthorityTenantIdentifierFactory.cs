using System.Text;

namespace Dotnettency
{
    public class RequestAuthorityTenantIdentifierFactory<TTenant> : HttpContextTenantIdentifierFactory<TTenant>
        where TTenant : class
    {
        public RequestAuthorityTenantIdentifierFactory(IHttpContextProvider httpContextAccessor) : base(httpContextAccessor)
        {
        }

        protected override TenantIdentifier GetTenantIdentifier(HttpContextBase context)
        {
            var uri = context.Request.GetUri();
            
            //var authorityUriString = new StringBuilder(uri.ToString().Length)
            //    .Append(uri.Scheme)
            //    .Append(SchemeDelimiter)
            //    .Append(uri.Host)
            //    //.Append(uri.PathAndQuery)
            //    //.Append(queryString)
            //    .ToString();

            var authorityUriBuilder = new System.UriBuilder(uri);
            authorityUriBuilder.Path = null;
            authorityUriBuilder.Query = null;
            return new TenantIdentifier(authorityUriBuilder.Uri);
        }
    }

}