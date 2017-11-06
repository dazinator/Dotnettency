using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Dotnettency.MiddlewarePipeline
{
    public interface ITenantPipelineAccessor<TTenant>
        where TTenant : class
    {
        Func<IApplicationBuilder, RequestDelegate, Lazy<Task<RequestDelegate>>> TenantPipeline { get; }
    }
}
