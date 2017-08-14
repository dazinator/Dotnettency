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

                var container = await containerFactory();

                container.Configure(async sharedServices =>
                {
                    await Task.WhenAll(allModules.Select(m => m.EnsureStarted(containerFactory, rootAppBuilder, sharedServices)));
                });

                // collate routers
                foreach (var module in allModules.Where(m => m.Router != null))
                {
                    this.ModulesRouter.AddModuleRouter(module);
                }

                // ModulesRouter = modulesRouter;
                Started = true;

            }
            finally
            {
                _semaphore.Release();
            }

        }

        public IRouter GetModulesRouter()
        {
            return ModulesRouter;
        }
    }
}
