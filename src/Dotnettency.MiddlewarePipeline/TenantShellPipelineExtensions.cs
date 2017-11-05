using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Dotnettency.MiddlewarePipeline
{
    public static class TenantShellPipelineExtensions
    {
        public static Lazy<Task<RequestDelegate>> GetOrAddMiddlewarePipeline<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<Task<RequestDelegate>> requestDelegateFactory)
            where TTenant : class
        {
            return tenantShell.Properties.GetOrAdd(nameof(TenantShellPipelineExtensions), requestDelegateFactory) as Lazy<Task<RequestDelegate>>;
        }
    }
}
