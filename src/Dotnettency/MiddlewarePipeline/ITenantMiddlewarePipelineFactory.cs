using System;
using System.Threading.Tasks;


namespace Dotnettency.MiddlewarePipeline
{
    public interface ITenantMiddlewarePipelineFactory<TTenant, TAppBuilder, TRequestDelegate>
     where TTenant : class
    {
        Task<TRequestDelegate> Create(TAppBuilder appBuilder, IServiceProvider serviceProviderOverride, TTenant tenant, TRequestDelegate next, bool reJoin);
    }

}
