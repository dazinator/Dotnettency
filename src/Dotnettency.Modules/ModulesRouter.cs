using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnettency.Modules
{
    public class ModulesRouter<TModule> : IRouter
    {
        public LinkedList<IModuleShell<TModule>> RoutedModules { get; set; }
        public IRouteHandler DefaultRouteHandler { get; set; }

        public ModulesRouter(IRouteHandler defaultRouteHandler)
        {
            DefaultRouteHandler = defaultRouteHandler;
            RoutedModules = new LinkedList<IModuleShell<TModule>>();
        }

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

                await module.Router.RouteAsync(moduleRouteContext);
                if (moduleRouteContext.Handler != null)
                {
                    var modulesRouteContext = context as ModulesRouteContext<TModule>;
                    var cast = currentNode.Value as IModuleShell<TModule>;
                    modulesRouteContext.ModuleShell = cast;

                    var existingRouteData = context.HttpContext.GetRouteData();
                    context.Handler = moduleRouteContext.Handler;
                    context.RouteData = moduleRouteContext.RouteData;

                    return;
                }
                else
                {
                    currentNode = currentNode.Next;
                }
            }
        }
    }
}
