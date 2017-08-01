using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{

    public class ModuleManager<TModule> : IModuleManager<TModule>
        where TModule : IModule
    {

        // private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ModuleManager()
        {
            Modules = new List<ModuleShell<TModule>>();
        }

        private List<ModuleShell<TModule>> Modules { get; set; }

        public void AddModule(ModuleShell<TModule> module)
        {
            Modules.Add(module);
        }

        public ModulesRouter<TModule> ModulesRouter { get; set; }

        public async Task EnsureModulesStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder)
        {
            var allModules = Modules.ToArray();

            var defaultRouteHandler = new RouteHandler(context =>
            {
                var routeValues = context.GetRouteData().Values;
                return context.Response.WriteAsync(
                    $"Hello! Route values: {string.Join(", ", routeValues)}");
            });

            var modulesRouter = new ModulesRouter<TModule>(rootAppBuilder, defaultRouteHandler);
            //todo: PERF: make the EnsureStarted task on each module a lazy<Task> so that its only run once and we dont create new tasks on every request.
            var startAllModules = allModules.Select(m => m.EnsureStarted(containerFactory, rootAppBuilder, modulesRouter));
            await Task.WhenAll(startAllModules);
            ModulesRouter = modulesRouter;
            //

            //foreach (var module in allModules)
            //{
            //    // TODO: Use await all to bootstrap modules in parralell!
            //    await EnsureModulesStarted(module, containerFactory, rootAppBuilder);
            //}
        }

        public ModulesRouter<TModule> GetModulesRouter()
        {
            return ModulesRouter;
        }
    }
}
