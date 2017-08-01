using Dotnettency.Modules;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency
{
    public static class UseModulesBuilderExtensions
    {
        public static MultitenancyMiddlewareOptionsBuilder<TTenant> UseModules<TTenant, TModule>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder)
            where TTenant : class
            where TModule : IModule
        {
            builder.ApplicationBuilder.UseMiddleware<ModulesMiddleware<TTenant, TModule>>(builder.ApplicationBuilder);
            return builder;
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