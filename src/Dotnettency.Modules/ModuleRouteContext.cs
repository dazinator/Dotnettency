using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{
    public class ModuleRouteContext : RouteContext
    {
        private readonly RouteContext _parentRouteContext;

        public bool NotMatched { get; set; }
        public RouteContext ParentRouteContext { get; set; }

        public ModuleRouteContext(HttpContext httpContext, RouteContext parentRouteContext) : base(httpContext)
        {
            _parentRouteContext = parentRouteContext;
            RouteData = _parentRouteContext.RouteData;
        }
    }
}
