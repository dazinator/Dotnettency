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
            var property = tenantShell.GetOrAddProperty(nameof(TenantShellPipelineExtensions), requestDelegateFactory);
            tenantShell.RegisterCallbackOnDispose(() =>
            {
                if (requestDelegateFactory.IsValueCreated)
                {
                    requestDelegateFactory.Value?.Dispose();
                }
            });
            return property;
        }
    }
}
