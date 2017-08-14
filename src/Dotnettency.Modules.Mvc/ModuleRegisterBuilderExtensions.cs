using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Modules
{
    public static class ModuleRegisterBuilderExtensions
    {

        public static IServiceCollection AddMvcModules<TModule>(this IServiceCollection servicies,
           Action<ModuleRegisterBuilder<TModule>> registerModules)
       where TModule : class, IModule
        {
            var routeHandler = DefaultRouteHandler ?? sp.GetRequiredService<MvcRouteHandler>();
            var registerModulesBuilder = new ModuleRegisterBuilder<TModule>(servicies, defaultRouteHandler);
            registerModules(registerModulesBuilder);
            return servicies;
        }

        public static ModuleRegisterBuilder<TModule> ConfigureMvc<TModule>(this ModuleRegisterBuilder<TModule> builder, Action<MvcOptions> mvcOptionsSetup = null)
            where TModule : class, IRoutedModule
        {
            if (mvcOptionsSetup != null)
            {
                builder.Services.AddMvc(mvcOptionsSetup);
            }
            else
            {
                builder.Services.AddMvc();
            }

            var routeHandler = sp.GetRequiredService<MvcRouteHandler>();

            return builder;

         
        }
    }



}
