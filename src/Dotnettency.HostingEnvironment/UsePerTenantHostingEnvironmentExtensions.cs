using System;

namespace Dotnettency
{
    public static class UsePerTenantHostingEnvironmentExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UsePerTenantHostingEnvironment<TTenant>(
            this MultitenancyMiddlewareOptionsBuilder<TTenant> builder,
            Action<PerTenantHostingEnvironmentMiddlewareOptionsBuilder<TTenant>> configure)
            where TTenant : class
        {
            var optionsBuilder = new PerTenantHostingEnvironmentMiddlewareOptionsBuilder<TTenant>(builder);
            configure(optionsBuilder);
            return builder;
        }
    }
}
