using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{
    public interface INancyModuleManager<TModule>
       where TModule : INancyModule
    {
        Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder);

        NancyModulesRouter<TModule> GetModulesRouter();
    }

    public interface INancyModuleShell<out TModule>
       where TModule : INancyModule
    {
        IApplicationBuilder AppBuilder { get; }
        ITenantContainerAdaptor Container { get; set; }
        bool IsStarted { get; }
        INancyModule Module { get; set; }
        ModuleShellOptions Options { get; }
        Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder, IServiceCollection sharedServices);
    }

    public interface IRoutedNancyModuleShell<out TModule> : INancyModuleShell<TModule>
         where TModule : INancyModule
    {
        RequestDelegate MiddlewarePipeline { get; set; }
        IRouter Router { get; }
    }

    public class RoutedNancyModuleShell<TModule> : IRoutedNancyModuleShell<TModule>
      where TModule : INancyModule
    {
        //private IRoutedModule routedModule;

        private NancyModulesRouter<TModule> _modulesRouter;

        public ModuleShellOptions Options { get; }

        public RoutedNancyModuleShell(TModule module, ModuleShellOptions options, NancyModulesRouter<TModule> modulesRouter)
        {
            Options = options;
            Module = module;
            _modulesRouter = modulesRouter;
        }

        public INancyModule Module { get; set; }

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

            var routedModuleShell = this as NancyModuleShell<IRoutedModule>;

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

    public class NancyModuleShell<TModule> : IModuleShell<TModule>
    where TModule : IModule
    {

        public ModuleShellOptions Options { get; }

        // public Func<IServiceCollection> ServicesFactory { get; }

        public NancyModuleShell(TModule module, ModuleShellOptions options)
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

    public class NancyModulesRouteContext<TModule> : RouteContext
      where TModule : INancyModule
        // where TTenant : class
    {
        //  private readonly RouteContext _parentRouteContext;

        public NancyModulesRouteContext(HttpContext httpContext) : base(httpContext)
        {
            //  NotMatched = false;
        }

        public INancyModuleShell<TModule> ModuleShell { get; set; }

        //public bool NotMatched { get; set; }

        // public RouteContext ParentRouteContext { get; set; }
    }

    public class NancyModuleManager<TModule> : INancyModuleManager<TModule>
        where TModule : INancyModule
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public bool Started { get; private set; }

        public NancyModuleManager(NancyModulesRouter<TModule> modulesRouter)
        {
            Modules = new List<INancyModuleShell<TModule>>();
            ModulesRouter = modulesRouter;
        }

        private List<INancyModuleShell<TModule>> Modules { get; set; }

        public void AddModule(INancyModuleShell<TModule> module)
        {
            Modules.Add(module);
        }

        public NancyModulesRouter<TModule> ModulesRouter { get; set; }

        public async Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder)
        {
            if (Started)
            {
                return;
            }

            await _semaphore.WaitAsync();
            try
            {
                if (Started)
                {
                    return;
                }

                var allModules = Modules.ToArray();

                //var defaultRouteHandler = new RouteHandler(context =>
                //{
                //    var routeValues = context.GetRouteData().Values;
                //    return context.Response.WriteAsync(
                //        $"Hello! Route values: {string.Join(", ", routeValues)}");
                //});

                // var modulesRouter = new ModulesRouter<IRoutedModule>(rootAppBuilder, defaultRouteHandler);
                //todo: PERF: make the EnsureStarted task on each module a lazy<Task> so that its only run once and we dont create new tasks on every request.
                // start shared modules first, as these are for libraries that add services and middleware that can impact downstream / routed modules.



                // allModules.Select(a=>a.Module).OfType<ISharedModule>();      

                var container = await containerFactory();

                container.Configure(async sharedServices =>
                {
                    await Task.WhenAll(allModules.Select(m => m.EnsureStarted(containerFactory, rootAppBuilder, sharedServices)));

                    //foreach (var item in allModules)
                    //{
                    //    var sharedModule = item as ModuleShell<ISharedModule>;
                    //    var routedModule = item as RoutedModuleShell<TModule>;

                    //}

                    //var sharedModules = allModules.OfType<IModuleShell<TModule>>().ToArray();
                    //if (sharedModules.Any())
                    //{


                    //}

                });


                // configure all ISharedModules into the same ServiceCollection to avoid duplicate registrations.



                // configure all IRoutedModules
                var routedModules = allModules.OfType<IModuleShell<IRoutedModule>>().ToArray();
                // routed modules can't add to tenant level services.
                await Task.WhenAll(routedModules.Select(m => m.EnsureStarted(containerFactory, rootAppBuilder, null)));

                // ModulesRouter = modulesRouter;
                Started = true;

            }
            finally
            {
                _semaphore.Release();
            }

        }

        public NancyModulesRouter<TModule> GetModulesRouter()
        {
            return ModulesRouter;
        }
    }
}
