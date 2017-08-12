using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantMiddlewarePipelineFactory<TTenant>
     where TTenant : class
    {
        Task<RequestDelegate> Get(IApplicationBuilder appBuilder, TTenant tenant, ITenantContainerAccessor<TTenant> tenantContainerAccessor, RequestDelegate next);
    }
}