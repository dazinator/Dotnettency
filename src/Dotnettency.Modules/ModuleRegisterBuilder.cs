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
        private IServiceCollection _services;
        public ModuleRegisterBuilder(IServiceCollection servicies)
        {
            _services = servicies;
            _services.AddRouting(); // needed for modular routing.
        }

        public ModuleRegisterBuilder<TModule> AddModule<TImplementation>()
            where TImplementation : class, TModule
        {
            _services.AddTransient<TModule, TImplementation>();
            return this;
        }

        public void OnSetupModule(Action<ModuleShellOptionsBuilder<TModule>> configureModuleOptionsBuilder, RouteHandler defaultRouteHandler)
        {
            //  var moduleShell = new
            var modulesRouter = new ModulesRouter<TModule>(defaultRouteHandler);
            //  _services.AddSingleton(modulesRouter);

            _services.AddSingleton<IModuleManager<TModule>, ModuleManager<TModule>>((sp) =>
        {
            var allModules = sp.GetServices<TModule>();

            var moduleManager = new ModuleManager<TModule>(modulesRouter);

            // shared modules all popualte the same service collection
            //   var services = new ServiceCollection();

            foreach (var item in allModules)
            {
                var moduleOptionsBuilder = new ModuleShellOptionsBuilder<TModule>(item);
                configureModuleOptionsBuilder(moduleOptionsBuilder);
                var moduleShellOptions = moduleOptionsBuilder.Build();

                var routedModule = item as IRoutedModule;
                if (routedModule != null) // these need to be routed.
                {
                    // modulesRouter.
                    // var routedModuleOptions = moduleShellOptions as ModuleShellOptions<IRoutedModule>;
                    var routedModuleShell = new RoutedModuleShell<TModule>(item, moduleShellOptions, modulesRouter);
                    // var moduleShell = routedModuleShell as IModuleShell<TModule>;
                    moduleManager.AddModule(routedModuleShell);
                }
                else
                {

                    var nonRoutedModuleShell = new ModuleShell<TModule>(item, moduleShellOptions);
                    //  var moduleShell = routedModuleShell as IModuleShell<TModule>;
                    moduleManager.AddModule(nonRoutedModuleShell);
                }

            }
            return moduleManager;
        });

        }
    }



}
