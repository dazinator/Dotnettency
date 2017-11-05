using DotNet.Cabinets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dotnettency.HostingEnvironment
{
    public class TenantHostingEnvironmentContentRootMiddleware<TTenant>
        where TTenant : class
    {
        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly ILogger<TenantHostingEnvironmentContentRootMiddleware<TTenant>> _logger;
        private readonly ITenantContentRootFileSystemProviderFatory<TTenant> _factory;
        
        public TenantHostingEnvironmentContentRootMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            ILogger<TenantHostingEnvironmentContentRootMiddleware<TTenant>> logger,
            ITenantContentRootFileSystemProviderFatory<TTenant> factory)
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
                _logger.LogDebug("Hosting Environment Middleware - Null tenant shell - No Tenant ContentRoot File Provider.");
                await _next(context);
                return;
            }

            var tenant = tenantShell?.Tenant;
            var tenantContentRootFileSystem = tenantShell.GetOrAddTenantContentRootFileSystem(new Lazy<ICabinet>(() =>
            {
                return _factory.GetContentRoot(tenant);
            }));

            // Swap out IFileProvider on IHostingEnvironment
            var oldContentRootFilePrvovider = hosting.ContentRootFileProvider;

            try
            {
                _logger.LogDebug("Hosting Environment Middleware - Swapping Content Root FileProvider.");
                hosting.ContentRootFileProvider = tenantContentRootFileSystem.Value.FileProvider;
                await _next(context);
            }
            finally
            {
                _logger.LogDebug("Hosting Environment Middleware - Restoring Content Root FileProvider.");
                hosting.ContentRootFileProvider = tenantContentRootFileSystem.Value.FileProvider;
            }              
        }
    }
}
