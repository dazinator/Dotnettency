using Dotnettency.Modules;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency
{
    public static class UseModulesBuilderExtensions
    {
        public static UseModulesOptionsBuilder<TTenant> UseModules<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder)
            where TTenant : class
        {
            return new UseModulesOptionsBuilder<TTenant>(builder);
        }

        public static IApplicationBuilder UseModules<TTenant, TModule>(this IApplicationBuilder builder)
            where TTenant : class
            where TModule : IModule
        {
            builder.UseMiddleware<ModulesMiddleware<TTenant, TModule>>(builder);
            return builder;
        }
    }
}
