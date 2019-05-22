using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dotnettency.AspNetCore.MiddlewarePipeline
{

    public class TenantPipelineMiddlewareOptions
    {
        public IApplicationBuilder RootApp { get; set; }

        public bool IsTerminal { get; set; }


    }
    public class TenantPipelineMiddleware<TTenant>
        where TTenant : class
    {
        private readonly RequestDelegate _next;
        private readonly TenantPipelineMiddlewareOptions _options;
        private readonly ILogger<TenantPipelineMiddleware<TTenant>> _logger;        

        public TenantPipelineMiddleware(
            RequestDelegate next,
            TenantPipelineMiddlewareOptions options,
            ILogger<TenantPipelineMiddleware<TTenant>> logger)
        {
            _next = next;
            _options = options;
            _logger = logger;          
        }

        public async Task Invoke(HttpContext context, ITenantPipelineAccessor<TTenant> tenantPipelineAccessor, ITenantMiddlewarePipelineFactory<TTenant> tenantPipelineFactory)
        {
            _logger.LogDebug("Tenant Pipeline Middleware - Getting Tenant Pipeline.");
            var tenantPipeline = await tenantPipelineAccessor.TenantPipeline(_options.RootApp, _options.RootApp.ApplicationServices, _next, tenantPipelineFactory, !_options.IsTerminal).Value;

            if (tenantPipeline != null)
            {
                _logger.LogDebug("Tenant Pipeline Middleware - Executing Pipeline.");
                await tenantPipeline(context);
            }
            else
            {
                _logger.LogDebug("Null tenant shell - No Tenant Middleware Pipeline to execute.");
                await _next(context);
            }
        }
    }
}
