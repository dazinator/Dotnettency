using Dotnettency.AspNetCore;
using Dotnettency.AspNetCore.Container;
using Microsoft.AspNetCore.Builder;

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
