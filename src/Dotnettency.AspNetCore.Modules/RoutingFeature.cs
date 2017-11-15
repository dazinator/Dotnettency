using Microsoft.AspNetCore.Routing;

namespace Dotnettency.AspNetCore.Modules
{
    public class RoutingFeature : IRoutingFeature
    {
        public RouteData RouteData { get; set; }
    }
}
