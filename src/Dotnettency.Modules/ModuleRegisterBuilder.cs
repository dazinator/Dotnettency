using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Modules
{
    public class ModuleRegisterBuilder<TModule>
        where TModule : class, IModule
    {
        public Func<IRouteHandler> DefaultRouteHandlerFactory { get; set; }
        public IServiceCollection Services { get; set; }

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

        public void ConfigureModules()
        {
            var moMatchRouteHandler = new RouteHandler(context =>
            {
                return null;
            });

            ConfigureModules(moMatchRouteHandler);          
        }

        public void ConfigureModules(IRouter moMatchRouteHandler)
        {
            OnSetupModule((moduleOptions) =>
            {
                // TODO: visitor design pattern might be better suite here..
                if (moduleOptions.Module is ISharedModule sharedModule)
                {
                    // Modules adds services to tenant level container.
                    moduleOptions.HasSharedServices((moduleServices) =>
                    {
                        sharedModule.ConfigureServices(moduleServices);
                    });
                    // Module adds middleware to tenant level pipeline.
                    moduleOptions.HasMiddlewareConfiguration((appBuilder) =>
                    {
                        sharedModule.ConfigureMiddleware(appBuilder);
                    });
                }

                // We allow IRoutedModules to partipate in configuring their own isolated services, associated with their router 
                if (moduleOptions.Module is IRoutedModule routedModule)
                {
                    // Module has its own container, that is associated with certain routes.
                    moduleOptions.HasRoutedContainer((moduleAppBuilder) =>
                    {
                        var moduleRouteBuilder = new RouteBuilder(moduleAppBuilder, moMatchRouteHandler);
                        routedModule.ConfigureRoutes(moduleRouteBuilder);
                        var moduleRouter = moduleRouteBuilder.Build();
                        return moduleRouter;
                    },
                    moduleServices => routedModule.ConfigureServices(moduleServices));
                }
            });
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
    }
}
