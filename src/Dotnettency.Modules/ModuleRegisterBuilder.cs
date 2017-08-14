using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Modules
{
    public class ModuleRegisterBuilder<TModule>
        where TModule : class, IModule
    {
        public Func<IRouteHandler> DefaultRouteHandlerFactory { get; set; }

        public ModuleRegisterBuilder(IServiceCollection servicies)
        {
            Services = servicies;
            DefaultRouteHandlerFactory = () =>
            {
                return new RouteHandler(context =>
                {
                    return null;
                });
            };            
            Services.AddRouting(); // needed for modular routing.
        }

        public ModuleRegisterBuilder<TModule> AddModule<TImplementation>()
            where TImplementation : class, TModule
        {
            Services.AddTransient<TModule, TImplementation>();
            return this;
        }

        public void OnSetupModule(Action<ModuleShellOptionsBuilder<TModule>> configureModuleOptionsBuilder)
        {          
            Services.AddSingleton<IModuleManager<TModule>, ModuleManager<TModule>>((sp) =>
            {
                var routeHandler = DefaultRouteHandlerFactory();
                var modulesRouter = new ModulesRouter<TModule>(routeHandler);

                var allModules = sp.GetServices<TModule>();

                var moduleManager = new ModuleManager<TModule>(modulesRouter);

                foreach (var item in allModules)
                {
                    var moduleOptionsBuilder = new ModuleShellOptionsBuilder<TModule>(item);
                    configureModuleOptionsBuilder(moduleOptionsBuilder);
                    var moduleShellOptions = moduleOptionsBuilder.Build();
                    var moduleShell = new ModuleShell<TModule>(item, moduleShellOptions);
                    moduleManager.AddModule(moduleShell);
                }
                return moduleManager;
            });
        }

        public IServiceCollection Services { get; set; }
    }
}