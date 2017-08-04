using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.Modules
{

    public class RoutedModuleShell<TModule> : IModuleShell<TModule>
       where TModule : IModule
    {
        //private IRoutedModule routedModule;

        private ModulesRouter<TModule> _modulesRouter;

        public ModuleShellOptions Options { get; }

        public RoutedModuleShell(TModule module, ModuleShellOptions options, ModulesRouter<TModule> modulesRouter)
        {
            Options = options;
            Module = module;
            _modulesRouter = modulesRouter;
        }

        public IModule Module { get; set; }

        public bool IsStarted { get; private set; }

        public ITenantContainerAdaptor Container { get; set; }
        public IApplicationBuilder AppBuilder { get; private set; }
        public IRouter Router { get; private set; }

        public RequestDelegate MiddlewarePipeline { get; set; }

        public async Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder, IServiceCollection sharedServices)
        {
            if (IsStarted)
            {
                return;
            }

            var container = await containerFactory();
            var routedModule = Module as IRoutedModule;
            container = container.CreateChildContainer();

            container.Configure((services) =>
            {
                services.AddRouting(); //it's assumed routing is required for a routed module!
                routedModule.ConfigureServices(services);
            });

            Container = container;

            var routedModuleShell = this as ModuleShell<IRoutedModule>;

            var moduleAppBuilder = rootAppBuilder.New();

            var moduleServicesProvider = Container.GetServiceProvider();
            moduleAppBuilder.ApplicationServices = moduleServicesProvider;

            var moduleRouteBuilder = new RouteBuilder(moduleAppBuilder, _modulesRouter.NullMatchRouteHandler);
            routedModule.ConfigureRoutes(moduleRouteBuilder);
            var moduleRouter = moduleRouteBuilder.Build();
            moduleRouteBuilder.ApplicationBuilder.UseRouter(moduleRouter);
            AppBuilder = moduleRouteBuilder.ApplicationBuilder;
            MiddlewarePipeline = AppBuilder.Build();

            this.Router = moduleRouter;

            // Must register this module with the module router.         
            _modulesRouter.AddModuleRouter(this);

            IsStarted = true;


        }
    }


    public class ModuleShell<TModule> : IModuleShell<TModule>
    where TModule : IModule
    {

        public ModuleShellOptions Options { get; }

       // public Func<IServiceCollection> ServicesFactory { get; }

        public ModuleShell(TModule module, ModuleShellOptions options)
        {
            Options = options;
            Module = module;
          //  ServicesFactory = servicesFactory;
            //Services = services;
        }

        public IModule Module { get; set; }

        public bool IsStarted { get; private set; }

        public ITenantContainerAdaptor Container { get; set; }
        public IApplicationBuilder AppBuilder { get; private set; }
        public IRouter Router { get; private set; }

        public RequestDelegate MiddlewarePipeline { get; set; }

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
            var sharedModule = Module as ISharedModule;
            if (sharedModule != null)
            {
               // var services = ServicesFactory();
                sharedModule.ConfigureServices(sharedServices);
                //container.Configure((services) =>
                //{

                //});
            }

            Container = container;

            // configure middleware.
            sharedModule.ConfigureMiddleware(rootAppBuilder);

            IsStarted = true;
        }


    }

}
