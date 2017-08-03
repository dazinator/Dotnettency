using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{

    public class ModuleManager<TModule> : IModuleManager<TModule>
        where TModule : IModule
    {

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public bool Started { get; private set; }

        public ModuleManager(ModulesRouter<TModule> modulesRouter)
        {
            Modules = new List<IModuleShell<TModule>>();
            ModulesRouter = modulesRouter;
        }

        private List<IModuleShell<TModule>> Modules { get; set; }

        public void AddModule(IModuleShell<TModule> module)
        {
            Modules.Add(module);
        }

        public ModulesRouter<TModule> ModulesRouter { get; set; }

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

        public ModulesRouter<TModule> GetModulesRouter()
        {
            return ModulesRouter;
        }
    }
}
