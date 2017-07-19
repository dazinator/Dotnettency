using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Dotnettency
{
    public interface ITenantMiddlewarePipelineFactory<TTenant>
     where TTenant : class
    {
        RequestDelegate Get(IApplicationBuilder appBuilder, TTenant tenant, RequestDelegate next);
    }
    //public interface ITenant
    //{

    //}




}
