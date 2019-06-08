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
        private readonly Action<TenantPipelineBuilderContext<TTenant>, IAppBuilder> _configuration;

        public DelegateTenantMiddlewarePipelineFactory(Action<TenantPipelineBuilderContext<TTenant>, IAppBuilder> configuration)
        {
            _configuration = configuration;
        }

        public async Task<AppFunc> Create(IAppBuilder appBuilder, IServiceProvider serviceProviderOverride, TTenant tenant, AppFunc next, bool reJoin)
        {
            return await UseTenantPipeline(appBuilder, serviceProviderOverride, tenant, next, reJoin);
        }

        protected virtual Task<AppFunc> UseTenantPipeline(IAppBuilder rootApp,  IServiceProvider serviceProviderOverride, TTenant tenant, AppFunc next, bool reJoin)
        {
            return Task.Run(() =>
            {
                var branchBuilder = rootApp.New();
                branchBuilder.Properties["ApplicationServices"] = serviceProviderOverride;
                //branchBuilder.ApplicationServices = serviceProviderOverride;
                var builderContext = new TenantPipelineBuilderContext<TTenant>
                {
                    Tenant = tenant
                };

                _configuration(builderContext, branchBuilder);

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
