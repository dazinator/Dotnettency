using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Modules
{
    public class RoutedModuleBase : ModuleBase, IRoutedModule
    {
        public RoutedModuleBase()
        {
        }

        public virtual void ConfigureRoutes(IRouteBuilder routes)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
