using Dotnettency.MiddlewarePipeline;
using Owin;
using System;
using System.Threading.Tasks;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;


namespace Dotnettency.Owin.MiddlewarePipeline
{
    public class DelegateTenantMiddlewarePipelineFactory<TTenant> : ITenantMiddlewarePipelineFactory<TTenant, IAppBuilder, AppFunc>
        where TTenant : class
    {
        private readonly Action<TenantShellItemBuilderContext<TTenant>, IAppBuilder> _configuration;

        public DelegateTenantMiddlewarePipelineFactory(Action<TenantShellItemBuilderContext<TTenant>, IAppBuilder> configuration)
        {
            _configuration = configuration;
        }

        public async Task<AppFunc> Create(IAppBuilder appBuilder, TenantShellItemBuilderContext<TTenant> context, AppFunc next, bool reJoin)
        {
            return await UseTenantPipeline(appBuilder, context, next, reJoin);
        }

        protected virtual Task<AppFunc> UseTenantPipeline(IAppBuilder rootApp, TenantShellItemBuilderContext<TTenant> context, AppFunc next, bool reJoin)
        {
            return Task.Run(() =>
            {
                var branchBuilder = rootApp.New();
                branchBuilder.Properties["ApplicationServices"] = context.Services;
                //branchBuilder.ApplicationServices = serviceProviderOverride;
               
                _configuration(context, branchBuilder);

                // register root pipeline at the end of the tenant branch
                if(next!=null && reJoin)
                {
                    branchBuilder.Use(typeof(ReJoinMiddleware), next); 
                }

                var result = (AppFunc)branchBuilder.Build(typeof(AppFunc));
                return result;
            });
        }
    }
}
