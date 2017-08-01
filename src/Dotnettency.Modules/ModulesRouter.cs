using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{

    public class ModulesRouter<TModule> : IRouter
        where TModule : IModule
    {

        private IApplicationBuilder _appBuilder;

        //public List<IModule> RoutedModules { get; set; }

        public ModulesRouter(IApplicationBuilder appBuilder, RouteHandler defaultRouteHandler)
        {
            DefaultRouteHandler = defaultRouteHandler;
            _appBuilder = appBuilder;
            RoutedModules = new LinkedList<ModuleShell<TModule>>();

            // if adding a new module, we want the first modules default route handler to be chained to the last, so that we can evaluate a null match.
            NullMatchRouteHandler = new RouteHandler(context =>
            {
                // context.Items["NUL"]
                return null;

                //context.GetRouteData().
                //var routeValues = context.GetRouteData().Values;
                //return context.Response.WriteAsync(
                //    $"Hello! Route values: {string.Join(", ", routeValues)}");
            });

        }

        public LinkedList<ModuleShell<TModule>> RoutedModules { get; set; }


        public void AddModuleRouter(Func<RouteBuilder, ModuleShell<TModule>> configureModuleRoutes, IServiceProvider moduleServicesProvider)
        {

            var appBuilder = _appBuilder.New();
            appBuilder.ApplicationServices = moduleServicesProvider;


            var routeBuilder = new RouteBuilder(appBuilder, NullMatchRouteHandler);
            var moduleShell = configureModuleRoutes(routeBuilder);

            var newNode = new LinkedListNode<ModuleShell<TModule>>(moduleShell);
            RoutedModules.AddLast(newNode);
            //// swap out the previous nodes defualt handler to a null handler.
            //var previous = newNode.Previous;
            //if (previous != null)
            //{
            //    newNode.Value.Router.de
            //}

        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            var currentNode = RoutedModules.First;
            while ((currentNode != null))
            {
                var module = currentNode.Value;
                var moduleRouter = module.Router;
                var virtulPath = moduleRouter.GetVirtualPath(context);
                if (virtulPath != null)
                {
                    return virtulPath;
                }
                currentNode = currentNode.Next;
            }
            return null;
        }

        public async Task RouteAsync(RouteContext context)
        {          

            var moduleRouteContext = new ModuleRouteContext(context.HttpContext, context);
            var currentNode = RoutedModules.First;
            while ((currentNode != null))
            {
                var module = currentNode.Value;
                await module.Router.RouteAsync(moduleRouteContext);
                if (moduleRouteContext.Handler != null)
                {
                    var modulesRouteContext = context as ModulesRouteContext<TModule>;
                    modulesRouteContext.ModuleShell = currentNode.Value;
                    context.Handler = moduleRouteContext.Handler;
                    return;
                }
                else
                {
                    currentNode = currentNode.Next;
                }
                //if (moduleRouteContext.NotMatched)
                //{
                //    currentNode = currentNode.Next;
                //    continue;
                //}


            }
        }

        public RouteHandler DefaultRouteHandler { get; set; }

        public RouteHandler NullMatchRouteHandler { get; set; }


    }

}
