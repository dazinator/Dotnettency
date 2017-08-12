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
        private IRouteHandler _defaultRouteHandler;

        public ModuleRegisterBuilder(IServiceCollection servicies, IRouteHandler defaultRouteHandler)
        {
            _services = servicies;
            _defaultRouteHandler = defaultRouteHandler;
            _services.AddRouting(); // needed for modular routing.
        }

        public ModuleRegisterBuilder<TModule> AddModule<TImplementation>()
            where TImplementation : class, TModule
        {
            _services.AddTransient<TModule, TImplementation>();
            return this;
        }

        public void OnSetupModule(Action<ModuleShellOptionsBuilder<TModule>> configureModuleOptionsBuilder)
        {
            //  var moduleShell = new
           

            var modulesRouter = new ModulesRouter<TModule>(_defaultRouteHandler);
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
                var moduleShell = new ModuleShell<TModule>(item, moduleShellOptions);
                moduleManager.AddModule(moduleShell);
            }
            return moduleManager;
        });

        }
    }



}
