using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Modules
{
    public class MvcModulesBuilder<TModule>
         where TModule : class, IRoutedModule
    {
        public MvcModulesBuilder(ModuleRegisterBuilder<TModule> parentBuilder, Action<Microsoft.AspNetCore.Mvc.MvcOptions> mvcOptionsSetup = null)
        {
            ParentBuilder = parentBuilder;
            if (mvcOptionsSetup != null)
            {
                parentBuilder.Services.AddMvc(mvcOptionsSetup);
            }
            else
            {
                parentBuilder.Services.AddMvc();
            }
            //  DefaultRouteHandler = new MvcRouteHandler
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultRouteHandler">The default route handler to use when no module route match.</param>
        /// <returns></returns>
        public ModuleRegisterBuilder<TModule> Build(Action<ModuleShellOptionsBuilder<TModule>> configureModuleOptionsBuilder, IRouteHandler defaultRouteHandler)
        {
            var services = ParentBuilder.Services;
            services.AddSingleton<IModuleManager<TModule>, ModuleManager<TModule>>((sp) =>
            {
                var routeHandler = DefaultRouteHandler ?? sp.GetRequiredService<MvcRouteHandler>();
                var allModules = sp.GetServices<TModule>();

                var modulesRouter = new ModulesRouter<TModule>(routeHandler);
                var moduleManager = new ModuleManager<TModule>(modulesRouter);

                // shared modules all popualte the same service collection
                //   var services = new ServiceCollection();

                foreach (var item in allModules)
                {
                    var moduleOptionsBuilder = new ModuleShellOptionsBuilder<TModule>(item);
                    configureModuleOptionsBuilder(moduleOptionsBuilder);
                    var moduleShellOptions = moduleOptionsBuilder.Build();

                    var routedModule = item as IRoutedModule;
                    if (routedModule != null) // these need to be routed.
                    {
                        // modulesRouter.
                        // var routedModuleOptions = moduleShellOptions as ModuleShellOptions<IRoutedModule>;
                        var routedModuleShell = new RoutedModuleShell<TModule>(item, moduleShellOptions, modulesRouter);
                        // var moduleShell = routedModuleShell as IModuleShell<TModule>;
                        moduleManager.AddModule(routedModuleShell);
                    }
                    else
                    {

                        var nonRoutedModuleShell = new ModuleShell<TModule>(item, moduleShellOptions);
                        //  var moduleShell = routedModuleShell as IModuleShell<TModule>;
                        moduleManager.AddModule(nonRoutedModuleShell);
                    }

                }
                return moduleManager;
            });

            return ParentBuilder;
        }

        public ModuleRegisterBuilder<TModule> ParentBuilder { get; set; }

    }



}
