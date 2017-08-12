using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{
    public class ModuleShell<TModule> : IModuleShell<TModule>
    {
        public ModuleShellOptions<TModule> Options { get; }
        // public Func<IServiceCollection> ServicesFactory { get; }

        public ModuleShell(TModule module, ModuleShellOptions<TModule> options)
        {
            Options = options;
            Module = module;
        }

        public TModule Module { get; set; }

        public bool IsStarted { get; private set; }

        public ITenantContainerAdaptor Container { get; set; }
        public IApplicationBuilder AppBuilder { get; private set; }

        public IRouter Router { get; set; }

        //internal async Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder, ModulesRouter<IRoutedModule> modulesRouter)
        //{
        //    if (IsStarted)
        //    {
        //        return;
        //    }

        //    var container = await containerFactory();
        //    var routedModule = Module as IRoutedModule;
        //    container = container.CreateChildContainer();

        //    container.Configure((services) =>
        //    {
        //        services.AddRouting(); //it's assumed routing is required for a routed module!
        //        routedModule.ConfigureServices(services);
        //    });

        //    Container = container;

        //    var routedModuleShell = this as ModuleShell<IRoutedModule>;
        //    // Must register this module with the module router.
        //    modulesRouter.AddModuleRouter((moduleRouteBuilder) =>
        //    {
        //        routedModule.ConfigureRoutes(moduleRouteBuilder);
        //        var moduleRouter = moduleRouteBuilder.Build();
        //        moduleRouteBuilder.ApplicationBuilder.UseRouter(moduleRouter);
        //        this.Router = moduleRouter;

        //        AppBuilder = moduleRouteBuilder.ApplicationBuilder;
        //        MiddlewarePipeline = AppBuilder.Build();

        //        return routedModuleShell;
        //    }, Container.GetServiceProvider());


        //    IsStarted = true;


        //}

        public async Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder, IServiceCollection sharedServices)
        {
            if (IsStarted)
            {
                return;
            }

            var container = await containerFactory();

            // configure container.           
            Options.OnConfigureSharedServices?.Invoke(sharedServices);

            if (Options.OnConfigureModuleServices != null)
            {
                container = container.CreateChildContainer();
                container.Configure((services) =>
                {
                    services.AddRouting(); //it's assumed routing is required for a routed module!
                    Options.OnConfigureModuleServices(services);
                });
            }

            Container = container;

            Options.OnConfigureMiddleware?.Invoke(rootAppBuilder);

            if (Options.GetRouter != null)
            {
                var moduleAppBuilder = rootAppBuilder.New();
                var moduleServicesProvider = Container.GetServiceProvider();
                moduleAppBuilder.ApplicationServices = moduleServicesProvider;
                this.Router = Options.GetRouter(moduleAppBuilder);
            }

            IsStarted = true;
        }


    }

}
