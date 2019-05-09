using Dotnettency.AspNetCore.Container;
using Dotnettency.Middleware;

namespace Dotnettency
{
    public static class MultitenancyMiddlewareOptionsBuilderExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UsePerTenantContainers<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder)
            where TTenant : class
        {
            builder.ApplicationBuilder.UseMiddleware<TenantContainerMiddleware<TTenant>>(builder.ApplicationBuilder);
            return builder;
        }
    }
}
