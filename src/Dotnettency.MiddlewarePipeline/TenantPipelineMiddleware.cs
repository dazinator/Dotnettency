using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dotnettency.MiddlewarePipeline
{
    public class TenantPipelineMiddleware<TTenant>
        where TTenant : class
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly ILogger<TenantPipelineMiddleware<TTenant>> _logger;

        public TenantPipelineMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            ILogger<TenantPipelineMiddleware<TTenant>> logger)
        {
            _next = next;
            _rootApp = rootApp;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ITenantPipelineAccessor<TTenant> tenantPipelineAccessor)
        {
            _logger.LogDebug("Tenant Pipeline Middleware - Getting Tenant Pipeline.");
            var tenantPipeline = await tenantPipelineAccessor.TenantPipeline(_rootApp, _next).Value;

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
