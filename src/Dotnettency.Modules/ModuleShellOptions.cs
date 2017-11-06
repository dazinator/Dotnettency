using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public class ModuleShellOptions<TModule>
    {
        public Action<IServiceCollection> OnConfigureSharedServices { get; set; }
        public Action<IServiceCollection> OnConfigureModuleServices { get; set; }
        public Action<IApplicationBuilder> OnConfigureMiddleware { get; set; }
        public Func<IApplicationBuilder, IRouter> GetRouter { get; set; }
        public IRouteHandler NoMatchingRouteHandler { get; set; } = new RouteHandler(context =>
        {
            return null;
        });
    }
}
