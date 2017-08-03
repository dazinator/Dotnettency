using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Modules
{
    public class RoutedModuleBase : ModuleBase, IRoutedModule
    {
        public RoutedModuleBase()
        {
            // IsSystemModule = false;
        }

        public virtual void ConfigureRoutes(IRouteBuilder routes)
        {
            //routes.MapGet("special", context =>
            //{
            //    return context.Response.WriteAsync("Isolated Module");
            //});
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            // throw new NotImplementedException();
        }
    }
}
