using Owin;
using System;

namespace Dotnettency.Owin.MiddlewarePipeline
{
    public class TenantPipelineMiddlewareOptions
    {
        public IAppBuilder RootApp { get; set; }

        public IHttpContextProvider HttpContextProvider { get; set; }

        public IServiceProvider ApplicationServices { get; set; }

        public bool IsTerminal { get; set; }

    }
}
