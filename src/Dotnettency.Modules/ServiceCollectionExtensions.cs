using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Modules
{

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModules<TModule>(this IServiceCollection servicies,
            IRouteHandler defaultRouteHandler,
            Action<ModuleRegisterBuilder<TModule>> registerModules)
        where TModule : class, IModule
        {
            var registerModulesBuilder = new ModuleRegisterBuilder<TModule>(servicies, defaultRouteHandler);
            registerModules(registerModulesBuilder);
            return servicies;
        }
    }
}
