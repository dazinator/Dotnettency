using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
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
        private List<IModuleShell<TModule>> _modules { get; set; }

        public bool Started { get; private set; }
        public ModulesRouter<TModule> ModulesRouter { get; set; }

        public ModuleManager(ModulesRouter<TModule> modulesRouter)
        {
            _modules = new List<IModuleShell<TModule>>();
            ModulesRouter = modulesRouter;
        }
        
        public void AddModule(IModuleShell<TModule> module)
        {
            _modules.Add(module);
        }

        public async Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder)
        {
            if (Started)
            {
                return;
            }

            await _semaphore.WaitAsync();
            try
            {
                // Double lock
                if (Started)
                {
                    return;
                }

                var allModules = _modules.ToArray();

                var container = await containerFactory();

                container.Configure(async sharedServices =>
                {
                    await Task.WhenAll(allModules.Select(m => m.EnsureStarted(containerFactory, rootAppBuilder, sharedServices)));
                });

                // Collate routers
                foreach (var module in allModules.Where(m => m.Router != null))
                {
                    ModulesRouter.AddModuleRouter(module);
                }

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
