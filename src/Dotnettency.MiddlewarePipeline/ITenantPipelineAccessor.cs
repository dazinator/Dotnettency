using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace Dotnettency.MiddlewarePipeline
{
    public interface ITenantPipelineAccessor<TTenant>
         where TTenant : class
    {
        Func<IApplicationBuilder, RequestDelegate, Lazy<Task<RequestDelegate>>> TenantPipeline { get; }
    }
}
