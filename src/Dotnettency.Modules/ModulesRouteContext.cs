using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{
    public class ModulesRouteContext<TModule> : RouteContext
    {

        public ModulesRouteContext(HttpContext httpContext) : base(httpContext)
        {
            //  NotMatched = false;
        }

        public IModuleShell<TModule> ModuleShell { get; set; }

    }

}
