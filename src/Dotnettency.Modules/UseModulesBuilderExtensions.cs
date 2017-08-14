using Dotnettency.Container.StructureMap;
using Dotnettency.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency
{
    public static class UseModulesBuilderExtensions
    {
        public static UseModulesOptionsBuilder<TTenant> UseModules<TTenant>(this MultitenancyMiddlewareOptionsBuilder<TTenant> builder)
            where TTenant : class
        {
            var optionsBuilder = new UseModulesOptionsBuilder<TTenant>(builder);
            return optionsBuilder;
        }

        public static IApplicationBuilder UseModules<TTenant, TModule>(this IApplicationBuilder builder)
          where TTenant : class
          where TModule : IModule
        {
          //  var container = builder.ApplicationServices;
       //     var resolved = container.GetRequiredService(typeof(IModuleManager<ModuleBase>));
            builder.UseMiddleware<ModulesMiddleware<TTenant, TModule>>(builder);
            return builder;
        }
    }

}