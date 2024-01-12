using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public static class TenantServiceProviderFactory<Tenant>
    {
        public static Func<IServiceCollection, IServiceProvider> Factory { get; set; }
    }
}
