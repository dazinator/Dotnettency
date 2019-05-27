using System;
using System.Threading.Tasks;
namespace Dotnettency.MiddlewarePipeline
{
    public interface ITenantPipelineAccessor<TTenant, TAppBuilder, TRequestDelegate>
       where TTenant : class
    {
        Func<TAppBuilder, IServiceProvider, TRequestDelegate, ITenantMiddlewarePipelineFactory<TTenant, TAppBuilder, TRequestDelegate>, bool, Lazy<Task<TRequestDelegate>>> TenantPipeline { get; }
    }
}
