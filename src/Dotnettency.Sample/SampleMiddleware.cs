using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Dotnettency;

namespace Sample
{
    public class SampleMiddleware<TTenant>
     where TTenant : class
    {

        private readonly RequestDelegate _next;
        private readonly IApplicationBuilder _rootApp;
        private readonly ILogger<SampleMiddleware<TTenant>> _logger;
        private readonly IHostingEnvironment _env;


        public SampleMiddleware(
            RequestDelegate next,
            IApplicationBuilder rootApp,
            ILogger<SampleMiddleware<TTenant>> logger,
            IHostingEnvironment env)

        {
            _next = next;
            _rootApp = rootApp;
            _logger = logger;
            _env = env;
        }


        public async Task Invoke(HttpContext context, ITenantShellAccessor<TTenant> tenantShellAccessor, IHostingEnvironment hosting)
        {

            var constructorHostingEnv = _env;
            var requestHostingEnv = hosting;

            _logger.LogDebug("Constructor env: " + _env.GetType().Name);
            _logger.LogDebug("Constructor env: " + hosting.GetType().Name);
            await _next(context);
        }
    }
}

