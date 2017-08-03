using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{
    public class ModuleRouteContext : RouteContext
    {
        private readonly RouteContext _parentRouteContext;

        public ModuleRouteContext(HttpContext httpContext, RouteContext parentRouteContext) : base(httpContext)
        {
            _parentRouteContext = parentRouteContext;
            this.RouteData = _parentRouteContext.RouteData;
            NotMatched = false;
        }

        public bool NotMatched { get; set; }

        public RouteContext ParentRouteContext { get; set; }
    }

}
