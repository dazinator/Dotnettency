using Owin;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
using Microsoft.Extensions.DependencyInjection;
using Dotnettency.MiddlewarePipeline;

namespace Dotnettency.Owin.MiddlewarePipeline
{
    public class TenantPipelineMiddleware<TTenant>
        where TTenant : class
    {
        private readonly AppFunc _next;
        private readonly TenantPipelineMiddlewareOptions _options;


        public TenantPipelineMiddleware(AppFunc next, TenantPipelineMiddlewareOptions options)
        {
            _next = next;
            _options = options;
        }


        public async Task Invoke(IDictionary<string, object> environment)
        {

            var provider = _options.HttpContextProvider;
            var requestServices = provider.GetCurrent().GetRequestServices();

            var accessor = requestServices.GetService<ITenantPipelineAccessor<TTenant, IAppBuilder, AppFunc>>();
            var factory = requestServices.GetService<ITenantMiddlewarePipelineFactory<TTenant, IAppBuilder, AppFunc>>();

            var tenantPipeline = await accessor.TenantPipeline(_options.RootApp, _options.ApplicationServices, _next, factory, !_options.IsTerminal).Value;

            if (tenantPipeline != null)
            {
                await tenantPipeline(environment);
            }
            else
            {
                await _next?.Invoke(environment);
            }
        }
    }
}
