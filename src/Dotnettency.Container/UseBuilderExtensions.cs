using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency
{

    //public class MultitenenancyContainerMiddlewareOptionsBuilder<TTenant>
    //    where TTenant : class
    //{
    //    private readonly MultitenancyMiddlewareOptionsBuilder<TTenant> _parentOptions;

    //    public MultitenenancyContainerMiddlewareOptionsBuilder(MultitenancyMiddlewareOptionsBuilder<TTenant> parentOptions)
    //    {
    //        _parentOptions = parentOptions;
    //    }
    //}

    public static class UseBuilderExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UsePerTenantContainers<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder)
            where TTenant : class
        {
            builder.ApplicationBuilder.UseMiddleware<TenantContainerMiddleware<TTenant>>(builder.ApplicationBuilder);
            return builder;
        }
    }
}

