using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{
    public class ModulesRouteContext<TModule> : RouteContext
       where TModule : IModule
        // where TTenant : class
    {
      //  private readonly RouteContext _parentRouteContext;

        public ModulesRouteContext(HttpContext httpContext) : base(httpContext)
        {
            //  NotMatched = false;
        }

        public IModuleShell<TModule> ModuleShell { get; set; }

        //public bool NotMatched { get; set; }

       // public RouteContext ParentRouteContext { get; set; }
    }

}
