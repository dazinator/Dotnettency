using DotNet.Cabinets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dotnettency.HostingEnvironment
{
    public class TenantHostingEnvironmentWebRootMiddleware<TTenant>
        where TTenant : class
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly ILogger<TenantHostingEnvironmentContentRootMiddleware<TTenant>> _logger;
        private readonly ITenantWebRootFileSystemProviderFatory<TTenant> _factory;

        public TenantHostingEnvironmentWebRootMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            ILogger<TenantHostingEnvironmentContentRootMiddleware<TTenant>> logger,
            ITenantWebRootFileSystemProviderFatory<TTenant> factory)

        {
            _next = next;
            _rootApp = rootApp;
            _logger = logger;
            _factory = factory;
        }

        public async Task Invoke(HttpContext context, ITenantShellAccessor<TTenant> tenantShellAccessor, IHostingEnvironment hosting)
        {
            var tenantShell = await tenantShellAccessor.CurrentTenantShell.Value;

            if (tenantShell == null)
            {
                _logger.LogDebug("Hosting Environment Middleware - Null tenant shell - No Tenant WebRoot File Provider.");
                await _next(context);
                return;
            }

            var tenant = tenantShell?.Tenant;

            var tenantFileSystem = tenantShell.GetOrAddTenantWebRootFileSystem(new Lazy<ICabinet>(() =>
            {
                return _factory.GetWebRoot(tenant);
            }));

            // Swap out IFileProvider on IHostingEnvironment
            var oldWebRootFilePrvovider = hosting.WebRootFileProvider;

            try
            {
                _logger.LogDebug("Hosting Environment Middleware - Swapping Web Root FileProvider.");
                hosting.WebRootFileProvider = tenantFileSystem.Value.FileProvider;
                await _next(context);
            }
            finally
            {
                _logger.LogDebug("Hosting Environment Middleware - Restoring Web Root FileProvider.");
                hosting.ContentRootFileProvider = tenantFileSystem.Value.FileProvider;
            }
        }
    }
}
