using Dotnettency.MiddlewarePipeline;
using Dotnettency.Owin;
using Dotnettency.Owin.MiddlewarePipeline;
using Microsoft.Extensions.DependencyInjection;
using Owin;
using System;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Dotnettency
{
    public static class MultitenancyServicesExtensions
    {
        public static MultitenancyOptionsBuilder<TTenant> AddOwin<TTenant>(this MultitenancyOptionsBuilder<TTenant> builder)
           where TTenant : class
        {
            // this service will only work when 
            //  IAppBuilder.UseRequestScopeContext() middleware has been activated.

            var provider = new HttpContextProvider();
            builder.HttpContextProvider = provider;
            builder.Services.AddSingleton<IHttpContextProvider>(provider);
            return builder;
        }       

        public static MultitenancyOptionsBuilder<TTenant> OwinPipeline<TTenant>(this TenantPipelineOptionsBuilder<TTenant> builder, Action<TenantPipelineBuilderContext<TTenant>, IAppBuilder> configuration)
           where TTenant : class
        {
            var factory = new DelegateTenantMiddlewarePipelineFactory<TTenant>(configuration);
            // builder.
            builder.MultitenancyOptions.Services.AddSingleton<ITenantMiddlewarePipelineFactory<TTenant, IAppBuilder, AppFunc>>(factory);
            builder.MultitenancyOptions.Services.AddScoped<ITenantPipelineAccessor<TTenant, IAppBuilder, AppFunc>, TenantPipelineAccessor<TTenant>>();
            return builder.MultitenancyOptions;
        }

    }
}