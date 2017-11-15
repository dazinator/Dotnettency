using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.AspNetCore.Modules.Nancy
{
    public class TestNancyRoutedModule : RoutedModuleBase
    {
        public TestNancyRoutedModule()
        {
            // IsSystemModule = false;
        }

        public override void ConfigureRoutes(IRouteBuilder routes)
        {

            RequestDelegate handler = (c) =>
            {
                var name = c.GetRouteValue("name");
                return c.Response.WriteAsync($"Hi {name}, from module: {this.GetType().Name}");
            };

            // routes.MapMiddlewareRoute

            //   routes.MapMiddlewareRoute
            routes.MapGet("hello/{name}", handler);

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // throw new NotImplementedException();
        }
    }
}
