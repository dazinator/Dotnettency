using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DotNet.Cabinets;
using Microsoft.AspNetCore.Hosting;

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
            if (tenantShell != null)
            {
                var tenant = tenantShell?.Tenant;
                var tenantContentRootFileSystem = tenantShell.GetOrAddTenantContentRootFileSystem<TTenant>(new Lazy<ICabinet>(() =>
                {
                    return _factory.GetContentRoot(tenant);
                }));

                // swap out IFileProvider on IHostingEnvironment
                var oldContentRootFilePrvovider = hosting.ContentRootFileProvider;
                try
                {
                    hosting.ContentRootFileProvider = tenantContentRootFileSystem.Value.FileProvider;
                    await _next(context);
                }
                finally
                {
                    hosting.ContentRootFileProvider = tenantContentRootFileSystem.Value.FileProvider;
                }              
            }
            else
            {
                _logger.LogDebug("Null tenant shell - No Tenant ContentRoot File Provider.");
                await _next(context);
            }

        }
    }
}
