using Dotnettency.Modules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Sample
{

    public class SampleRoutedModule : RoutedModuleBase
    {
        public SampleRoutedModule()
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

            routes.MapGet("hello/{name}", handler);

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // throw new NotImplementedException();
        }
    }
}
