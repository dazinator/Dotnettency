using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Modules
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModules<TModule>(this IServiceCollection services,
            Action<ModuleRegisterBuilder<TModule>> registerModules)
            where TModule : class, IModule
        {
            var registerModulesBuilder = new ModuleRegisterBuilder<TModule>(services);
            registerModules(registerModulesBuilder);
            return services;
        }
    }
}
