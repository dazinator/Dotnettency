using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Dotnettency.AspNetCore.MiddlewarePipeline
{
    public interface ITenantPipelineAccessor<TTenant>
        where TTenant : class
    {
        Func<IApplicationBuilder, IServiceProvider, RequestDelegate, ITenantMiddlewarePipelineFactory<TTenant>, Lazy<Task<RequestDelegate>>> TenantPipeline { get; }
    }
}
