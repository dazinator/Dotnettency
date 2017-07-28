using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DotNet.Cabinets;
using Microsoft.AspNetCore.Hosting;

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
            if (tenantShell != null)
            {
                var tenant = tenantShell?.Tenant;
                var tenantFileSystem = tenantShell.GetOrAddTenantWebRootFileSystem<TTenant>(new Lazy<ICabinet>(() =>
                {
                    return _factory.GetWebRoot(tenant);
                }));

                // swap out IFileProvider on IHostingEnvironment
                var oldWebRootFilePrvovider = hosting.WebRootFileProvider;
                try
                {
                    hosting.WebRootFileProvider = tenantFileSystem.Value.FileProvider;
                    await _next(context);
                }
                finally
                {
                    hosting.ContentRootFileProvider = tenantFileSystem.Value.FileProvider;
                }
            }
            else
            {
                _logger.LogDebug("Null tenant shell - No Tenant Web Root File Provider.");
                await _next(context);
            }
        }
    }
}
