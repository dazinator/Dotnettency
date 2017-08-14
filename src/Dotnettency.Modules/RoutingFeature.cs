using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{
    public class RoutingFeature : IRoutingFeature
    {
        public RouteData RouteData { get; set; }
    }
}
