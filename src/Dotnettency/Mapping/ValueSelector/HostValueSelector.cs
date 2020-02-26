namespace Dotnettency
{
    public class HostValueSelector : IHttpContextValueSelector
    {
        public string SelectValue(HttpContextBase httpContext)
        {
           // authorityUriBuilder.Host
            return httpContext?.Request?.GetUri()?.Host;
        }
    }
}