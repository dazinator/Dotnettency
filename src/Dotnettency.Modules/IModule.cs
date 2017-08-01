using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.AspNetCore.Routing;

namespace Dotnettency.Modules
{
    public interface IModule
    {
        void ConfigureServices(IServiceCollection services);
        void ConfigureRoutes(IRouteBuilder routes);
        void ConfigureMiddleware(IApplicationBuilder appBuilder);
    }
    
}
