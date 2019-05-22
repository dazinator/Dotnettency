using System;
using Dotnettency.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency.AspNetCore
{

    public class ApplicationBuilderAdaptor : AppBuilderAdaptorBase
    {
        private readonly IApplicationBuilder _appBuilder;

        public ApplicationBuilderAdaptor(IApplicationBuilder appBuilder)
        {
            _appBuilder = appBuilder;
        }

        public override IServiceProvider ApplicationServices { get => _appBuilder.ApplicationServices; set => _appBuilder.ApplicationServices = value; }

        public override void UseMiddleware<TMiddleware>()
        {
            _appBuilder.UseMiddleware<TMiddleware>();
        }
        public override void UseMiddleware<TMiddleware>(params object[] args)
        {
            _appBuilder.UseMiddleware<TMiddleware>(args);
        }
    }   


}