using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantMiddlewarePipelineFactory<TTenant>
        where TTenant : class
    {
        Task<RequestDelegate> Create(IApplicationBuilder appBuilder, TTenant tenant, RequestDelegate next);
    }
}
