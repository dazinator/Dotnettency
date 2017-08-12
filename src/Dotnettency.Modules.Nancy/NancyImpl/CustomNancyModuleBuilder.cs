using System;
using System.Threading.Tasks;

namespace Dotnettency.Modules.Nancy
{
    ///// <summary>
    ///// Default implementation for building a full configured <see cref="INancyModule"/> instance.
    ///// </summary>
    //public class CustomNancyModuleBuilder : INancyModuleBuilder
    //{

    //    private readonly IViewFactory viewFactory;
    //    private readonly IResponseFormatterFactory responseFormatterFactory;
    //    private readonly IModelBinderLocator modelBinderLocator;
    //    private readonly IModelValidatorLocator validatorLocator;
    //    private readonly IRouteBuilder routeBuilder;

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="DefaultNancyModuleBuilder"/> class.
    //    /// </summary>
    //    /// <param name="viewFactory">The <see cref="IViewFactory"/> instance that should be assigned to the module.</param>
    //    /// <param name="responseFormatterFactory">An <see cref="IResponseFormatterFactory"/> instance that should be used to create a response formatter for the module.</param>
    //    /// <param name="modelBinderLocator">A <see cref="IModelBinderLocator"/> instance that should be assigned to the module.</param>
    //    /// <param name="validatorLocator">A <see cref="IModelValidatorLocator"/> instance that should be assigned to the module.</param>
    //    public CustomNancyModuleBuilder(IViewFactory viewFactory,
    //        IResponseFormatterFactory responseFormatterFactory,
    //        IModelBinderLocator modelBinderLocator,
    //        IModelValidatorLocator validatorLocator,
    //        IRouteBuilder routeBuilder)
    //    {

    //        this.viewFactory = viewFactory;
    //        this.responseFormatterFactory = responseFormatterFactory;
    //        this.modelBinderLocator = modelBinderLocator;
    //        this.validatorLocator = validatorLocator;
    //        this.routeBuilder = routeBuilder;
    //    }



    //    /// <summary>
    //    /// Builds a fully configured <see cref="INancyModule"/> instance, based upon the provided <paramref name="module"/>.
    //    /// </summary>
    //    /// <param name="module">The <see cref="INancyModule"/> that should be configured.</param>
    //    /// <param name="context">The current request context.</param>
    //    /// <returns>A fully configured <see cref="INancyModule"/> instance.</returns>
    //    public INancyModule BuildModule(INancyModule module, NancyContext context)
    //    {
    //        module.Context = context;
    //        module.Response = this.responseFormatterFactory.Create(context);
    //        module.ViewFactory = this.viewFactory;
    //        module.ModelBinderLocator = this.modelBinderLocator;
    //        module.ValidatorLocator = this.validatorLocator;

    //        IApplicationBuilder appBuilder = GetAppBuilder();
    //        INancyModule[] nancyModules = GetNancyModules();

    //        foreach (var nancyModule in nancyModules)
    //        {
    //            var customRouter = ConvertNancyModuleRoutesToAspNetCoreRouter(module.Routes.ToArray(), appBuilder);
    //        }

    //        return module;
    //    }

    //    private INancyModule[] GetNancyModules()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private IApplicationBuilder GetAppBuilder()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    private CustomRouter ConvertNancyModuleRoutesToAspNetCoreRouter(Nancy.Routing.Route[] routes, IApplicationBuilder appBuilder)
    //    {


    //        var router = new CustomRouter(routes, appBuilder);
    //        return router;
    //    }


    //    public class CustomRouter : IRouter
    //    {
    //        private Nancy.Routing.Route[] _routes;
    //        private IApplicationBuilder _appBuilder;

    //        public Nancy.Routing.Route MatchedRoute { get; set; }

    //        public INancyModule MatchedModule { get; set; }

    //        public CustomRouter(Nancy.Routing.Route[] routes, IApplicationBuilder appBuilder)
    //        {
    //            _routes = routes;
    //            _appBuilder = appBuilder;

    //            // create a new owin pipeline for the nancy module to operate within.
    //            var owinBridge = appBuilder.UseOwin(addToPipeline =>
    //            {
    //                addToPipeline(next =>
    //                {
    //                    var owinAppBuilder = new AppBuilder();
    //                    owinAppBuilder.Properties["builder.DefaultApp"] = next;
    //                    owinAppBuilder.UseNancy();
    //                    // Uses the owin middleware.
    //                    owinAppBuilder.UseNancy((options) =>
    //                    {
    //                        options.Bootstrapper = new CustomBootstrapper(() =>
    //                        {
    //                            return new AlreadyKnownRouteResolver(null, this.MatchedRoute, this.MatchedModule)
    //                        });
    //                    });
    //                    return owinAppBuilder.Build<AppFunc>();

    //                });

    //            }).Build();

    //        }

    //        public Microsoft.AspNetCore.Http.RequestDelegate OwinBridge { get; internal set; }

    //        public VirtualPathData GetVirtualPath(VirtualPathContext context)
    //        {
    //            throw new NotImplementedException();
    //        }

    //        public Task RouteAsync(RouteContext context)
    //        {
    //            var result = FindMatchingRoute(context, _routes);
    //            if (result != null)
    //            {
    //                context.Handler =
    //            }

    //        }

    //        public int MyProperty { get; set; }

    //        private object FindMatchingRoute(RouteContext context, Nancy.Routing.Route[] routes)
    //        {
    //            throw new NotImplementedException();
    //        }


    //    }

    //}
}