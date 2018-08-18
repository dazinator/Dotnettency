using Microsoft.AspNetCore.Authentication;

namespace Sample.TenantAuthentication
{
    public partial class Startup
    {
        public class BasicAuthenticationOptions : AuthenticationSchemeOptions
        {
            public string Realm { get; set; }
        }
    }
}
