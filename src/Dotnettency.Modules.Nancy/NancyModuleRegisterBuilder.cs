using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using System;

namespace Dotnettency.Modules
{
    public class NancyModuleRegisterBuilder<TModule>
        where TModule : class, INancyModule
    {
        private IServiceCollection _services;
        public NancyModuleRegisterBuilder(IServiceCollection servicies)
        {
            _services = servicies;
            _services.AddRouting(); // needed for modular routing.
        }

        public NancyModuleRegisterBuilder<TModule> AddModule<TImplementation>()
            where TImplementation : class, TModule
        {
            _services.AddTransient<TModule, TImplementation>();
            return this;
        }

        public void OnSetupModule(Action<NancyModuleShellOptionsBuilder<TModule>> configureModuleOptionsBuilder, RouteHandler defaultRouteHandler)
        {
            //  var moduleShell = new
            var modulesRouter = new NancyModulesRouter<TModule>(defaultRouteHandler);
            //  _services.AddSingleton(modulesRouter);

            _services.AddSingleton<INancyModuleManager<TModule>, NancyModuleManager<TModule>>((sp) =>
        {
            var allModules = sp.GetServices<TModule>();
            var moduleManager = new NancyModuleManager<TModule>(modulesRouter);

            // shared modules all popualte the same service collection
            //   var services = new ServiceCollection();
            foreach (var item in allModules)
            {
                var moduleOptionsBuilder = new NancyModuleShellOptionsBuilder<TModule>(item);
                configureModuleOptionsBuilder(moduleOptionsBuilder);
                var moduleShellOptions = moduleOptionsBuilder.Build();

                var routedModule = item as IRoutedModule;
                if (routedModule != null) // these need to be routed.
                {
                    // modulesRouter.
                    // var routedModuleOptions = moduleShellOptions as ModuleShellOptions<IRoutedModule>;
                    var routedModuleShell = new RoutedNancyModuleShell<TModule>(item, moduleShellOptions, modulesRouter);
                    // var moduleShell = routedModuleShell as IModuleShell<TModule>;
                    moduleManager.AddModule(routedModuleShell);
                }
                else
                {

                    var nonRoutedModuleShell = new NancyModuleShell<TModule>(item, moduleShellOptions);
                    //  var moduleShell = routedModuleShell as IModuleShell<TModule>;
                    moduleManager.AddModule(nonRoutedModuleShell);
                }

            }
            return moduleManager;
        });

        }
    }



}
