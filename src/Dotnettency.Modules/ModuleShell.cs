using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{
    public class ModuleShell<TModule> : IModuleShell<TModule>
    {
        public ModuleShellOptions<TModule> Options { get; private set; }
        public TModule Module { get; set; }
        public bool IsStarted { get; private set; }
        public ITenantContainerAdaptor Container { get; set; }
        public IApplicationBuilder AppBuilder { get; private set; }
        public IRouter Router { get; set; }

        public ModuleShell(TModule module, ModuleShellOptions<TModule> options)
        {
            Options = options;
            Module = module;
        }

        public async Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder, IServiceCollection sharedServices)
        {
            if (IsStarted)
            {
                return;
            }

            var container = await containerFactory();

            // Configure container.           
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
                var moduleServicesProvider = Container;
                moduleAppBuilder.ApplicationServices = moduleServicesProvider;
                Router = Options.GetRouter(moduleAppBuilder);
            }

            IsStarted = true;
        }
    }
}
