using System;
using System.Threading.Tasks;
using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

namespace Dotnettency.Owin.MiddlewarePipeline
{
    public static class TenantShellPipelineExtensions
    {
        public static Lazy<Task<AppFunc>> GetOrAddMiddlewarePipeline<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<Task<AppFunc>> requestDelegateFactory)
            where TTenant : class
        {
            return tenantShell.Properties.GetOrAdd(nameof(TenantShellPipelineExtensions), requestDelegateFactory) as Lazy<Task<AppFunc>>;
        }
    }
}
