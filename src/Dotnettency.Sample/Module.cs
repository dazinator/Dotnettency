using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Dotnettency.Modules;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Sample
{
    public abstract class BaseModule : IModule
    {
        public abstract void ConfigureServices(IServiceCollection services);

        public abstract void ConfigureMiddleware(IApplicationBuilder appBuilder);

        public virtual void ConfigureRoutes(IRouteBuilder routes)
        {

        }

        public bool IsSystemModule { get; set; }

    }
    public class SystemModule : BaseModule
    {
        public SystemModule()
        {
            IsSystemModule = true;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            //  throw new NotImplementedException();
        }

        public override void ConfigureMiddleware(IApplicationBuilder appBuilder)
        {
            // throw new NotImplementedException();
        }
    }

    public class IsolatedModule : BaseModule
    {
        public IsolatedModule()
        {
            IsSystemModule = false;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public override void ConfigureRoutes(IRouteBuilder routes)
        {
            routes.MapGet("special", context =>
{
    return context.Response.WriteAsync("Isolated Module");
});

            //   routes.MapRoute("special", "special/thing");
            //   routes.MapRoute("default", "{controller=Home}/{action=Index}/{Id?}");
        }

        public override void ConfigureMiddleware(IApplicationBuilder appBuilder)
        {

            //appBuilder.UseRouter((routeBuilder) =>
            //{
            //    routeBuilder.DefaultHandler = defaultRouteHandler;
            //    routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{Id?}");
            //});
            //    throw new NotImplementedException();
        }
    }
}
