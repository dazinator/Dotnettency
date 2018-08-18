using Microsoft.Extensions.Options;
using System;

namespace Sample.TenantAuthentication
{
    public partial class Startup
    {
        public class BasicAuthenticationPostConfigureOptions : IPostConfigureOptions<BasicAuthenticationOptions>
        {
            public void PostConfigure(string name, BasicAuthenticationOptions options)
            {
                if (string.IsNullOrEmpty(options.Realm))
                {
                    throw new InvalidOperationException("Realm must be provided in options");
                }
            }
        }
    }
}
