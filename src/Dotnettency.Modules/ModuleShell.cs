using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.Modules
{
    public class ModuleShell<TModule>
        where TModule : IModule
    {

        public ModuleShellOptions<TModule> Options { get; }

        public ModuleShell(TModule module, ModuleShellOptions<TModule> options)
        {
            Options = options;
            Module = module;
        }

        public TModule Module { get; set; }

        public bool IsStarted { get; private set; }

        public ITenantContainerAdaptor Container { get; set; }
        public IApplicationBuilder AppBuilder { get; private set; }
        public IRouter Router { get; private set; }

        public RequestDelegate MiddlewarePipeline { get; set; }

        internal async Task EnsureStarted(Func<Task<ITenantContainerAdaptor>> containerFactory, IApplicationBuilder rootAppBuilder, ModulesRouter<TModule> modulesRouter)
        {
            if (IsStarted)
            {
                return;
            }

            var container = await containerFactory();
            if (Options.IsIsolated) // means module is routable and isolated. Services are registered into container per module, and module can configure its own middleware.
            {
                container = container.CreateChildContainer();
            }

            container.Configure((services) =>
            {
                if (Options.IsIsolated)
                {
                    services.AddRouting();
                }
                Module.ConfigureServices(services);
            });

            Container = container;


            // Configure routes.
            // only isolated modules can have routing.
            if (Options.IsIsolated)
            {
                modulesRouter.AddModuleRouter((moduleRouteBuilder) =>
                {
                    //  var appBuilder = rootAppBuilder.New();
                    //appBuilder.ApplicationServices = Container.GetServiceProvider();
                    //  var routeBuilder = new RouteBuilder(appBuilder, modulesRouter);
                    Module.ConfigureRoutes(moduleRouteBuilder);

                    var moduleRouter = moduleRouteBuilder.Build();
                    moduleRouteBuilder.ApplicationBuilder.UseRouter(moduleRouter);
                    this.Router = moduleRouter;

                    //TODO: Not sure it makes sense for an isolated module to have its own middleware pipeline.
                    // As means evauating routes twice - i.e we evaluate the routes to route to the correct module,
                    // then because the router is in the middleware pipeline if we invoke the middleware pipeline
                    // we will evaulate the routes again.
                    // perhaps we don't add the routing to the modules middleware pipeline, but then
                   

                    AppBuilder = moduleRouteBuilder.ApplicationBuilder;
                    MiddlewarePipeline = AppBuilder.Build();

                    return this;
                }, Container.GetServiceProvider());

            }
            else
            {
                // non isolated modules can directly configure middleware in the tenants pipeline.
                Module.ConfigureMiddleware(rootAppBuilder);
            }

            IsStarted = true;


        }
    }

}
