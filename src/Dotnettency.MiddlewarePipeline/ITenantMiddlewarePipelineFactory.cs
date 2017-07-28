using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;

namespace Dotnettency
{
    public interface ITenantMiddlewarePipelineFactory<TTenant>
     where TTenant : class
    {
        RequestDelegate Get(IApplicationBuilder appBuilder, TTenant tenant, IServiceProvider requestServices, RequestDelegate next);
    }
}