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

        public void OnSetupModule(Action<ModuleShellOptionsBuilder<TModule>> configureModuleOptionsBuilder)
        {
            //  var moduleShell = new
            _services.AddSingleton<IModuleManager<TModule>, ModuleManager<TModule>>((sp) =>
        {
            var allModules = sp.GetServices<TModule>();
            var moduleManager = new ModuleManager<TModule>();
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
