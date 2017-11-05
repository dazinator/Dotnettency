using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Modules
{
    /// <summary>
    /// An <see cref="IRoutedModule"/> is given its own container that it can configure with services, so that they are isolated from any other modules.
    /// The <see cref="IRoutedModule"/> must configure some Routes in it's <see cref="IRoutedModule.ConfigureRoutes(IRouteBuilder)"/> method, as these are the routes
    /// under which this modules container will be restored into RequestServices for an incoming request.
    /// </summary>
    /// <remarks>
    /// During an incoming request the <see cref="ModulesRouter{TModule}"></see>
    /// from the <see cref="ModulesMiddleware{TTenant, TModule}"></see> routes to each <see cref="IRoutedModule"/>'s <see cref="IRouter"/> and if it finds one that can handle the current request it then
    /// restores that modules child container into <see cref="System.Net.Http.HttpContext"/> RequestServices.
    /// A container with no routes is not valid, because it can never be restored during routing of an incoming request.
    /// </remarks>
    public interface IRoutedModule : IModule
    {
        void ConfigureRoutes(IRouteBuilder routes);
        void ConfigureServices(IServiceCollection services);
    }
}
