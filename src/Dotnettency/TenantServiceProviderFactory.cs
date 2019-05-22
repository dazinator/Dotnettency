using System;

namespace Dotnettency
{
    public static class TenantServiceProviderFactory<Tenant>
    {
        public static Func<IServiceProvider> Factory { get; set; }
    }
}
