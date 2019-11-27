using System;
using System.Threading.Tasks;


namespace Dotnettency.MiddlewarePipeline
{
    public interface ITenantMiddlewarePipelineFactory<TTenant, TAppBuilder, TRequestDelegate>
     where TTenant : class
    {
        Task<TRequestDelegate> Create(TAppBuilder appBuilder, TenantShellItemBuilderContext<TTenant> context, TRequestDelegate next, bool reJoin);
    }

}
