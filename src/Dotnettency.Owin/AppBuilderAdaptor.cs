using System;
using DavidLievrouw.OwinRequestScopeContext;
using Dotnettency.Middleware;
using Owin;

namespace Dotnettency.Owin
{

    public class AppBuilderAdaptor : AppBuilderAdaptorBase
    {
        private readonly IAppBuilder _appBuilder;

        public AppBuilderAdaptor(IAppBuilder appBuilder, IServiceProvider serviceProvider)
        {
            _appBuilder = appBuilder;
            ApplicationServices = serviceProvider;           
        }

        public override IServiceProvider ApplicationServices { get; set; }

        public override void UseMiddleware<TMiddleware>(params object[] args)
        {
            _appBuilder.Use(typeof(TMiddleware), args);
        }

        public override void UseMiddleware<TMiddleware>()
        {
            _appBuilder.Use(typeof(TMiddleware));
        }
    }   
}