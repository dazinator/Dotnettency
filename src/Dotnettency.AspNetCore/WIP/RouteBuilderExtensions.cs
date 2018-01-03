using Microsoft.AspNetCore.Routing;

namespace Sample
{
    public static class RouteBuilderExtensions
    {
        public static void MapTenant(this IRouteBuilder routeBuilder, string name, ITenantDistinguisherFactory tenantDistinguisher)
        {
            routeBuilder.Routes.Add(new TenantRouter(name));
        }
    }
}
