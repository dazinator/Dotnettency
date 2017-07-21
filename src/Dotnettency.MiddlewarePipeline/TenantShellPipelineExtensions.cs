using System;
using Microsoft.AspNetCore.Http;

namespace Dotnettency.MiddlewarePipeline
{
    public static class TenantShellPipelineExtensions
    {
        public static Lazy<RequestDelegate> GetOrAddMiddlewarePipeline<TTenant>(this TenantShell<TTenant> tenantShell, Lazy<RequestDelegate> requestDelegateFactory)
            where TTenant : class
        {
            var result = tenantShell.Properties.GetOrAdd(nameof(TenantShellPipelineExtensions), requestDelegateFactory) as Lazy<RequestDelegate>;
            return result;
        }
    }
}
