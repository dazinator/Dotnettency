using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{
    public class ModulesRouter<TModule> : IRouter
    {
        public ModulesRouter(IRouteHandler defaultRouteHandler)
        {
            DefaultRouteHandler = defaultRouteHandler;
            //  _appBuilder = appBuilder;
            RoutedModules = new LinkedList<IModuleShell<TModule>>();
            // NullMatchRouteHandler = nullMatchRouteHandler;
        }

        public LinkedList<IModuleShell<TModule>> RoutedModules { get; set; }

        public void AddModuleRouter(IModuleShell<TModule> routedModuleShell)
        {
            var newNode = new LinkedListNode<IModuleShell<TModule>>(routedModuleShell);
            RoutedModules.AddLast(routedModuleShell);
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

                // context.HttpContext.GetRouteData().Routers.Add(router);
                await module.Router.RouteAsync(moduleRouteContext);
                if (moduleRouteContext.Handler != null)
                {
                    var modulesRouteContext = context as ModulesRouteContext<TModule>;
                    var cast = currentNode.Value as IModuleShell<TModule>;
                    modulesRouteContext.ModuleShell = cast;

                    var existingRouteData = context.HttpContext.GetRouteData();
                    context.Handler = moduleRouteContext.Handler;
                    context.RouteData = moduleRouteContext.RouteData;

                    //  existingRouteData.PushState(module.Router, context.RouteData.Values, context.RouteData.DataTokens);
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

        public IRouteHandler DefaultRouteHandler { get; set; }

        //  public IRouteHandler NullMatchRouteHandler { get; set; }
    }
}