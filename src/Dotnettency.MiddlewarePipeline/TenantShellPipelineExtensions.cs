using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dotnettency.MiddlewarePipeline
{
    public static class TenantShellPipelineExtensions
    {
        public static Lazy<Task<RequestDelegate>> GetOrAddMiddlewarePipeline<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<Task<RequestDelegate>> requestDelegateFactory)
            where TTenant : class
        {
            var result = tenantShell.Properties.GetOrAdd(nameof(TenantShellPipelineExtensions), requestDelegateFactory) as Lazy<Task<RequestDelegate>>;
            return result;
        }
    }
}
