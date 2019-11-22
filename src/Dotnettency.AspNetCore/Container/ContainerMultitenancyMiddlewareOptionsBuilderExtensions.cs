using Dotnettency.AspNetCore.Container;
using Dotnettency.Middleware;

namespace Dotnettency
{
    public static class ContainerMultitenancyMiddlewareOptionsBuilderExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UseTenantContainers<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder)
            where TTenant : class
        {
            builder.ApplicationBuilder.UseMiddleware<TenantContainerMiddleware<TTenant>>(builder.ApplicationBuilder);
            return builder;
        }
    }

  
}

