using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Modules
{
    public interface IModule
    {
        void ConfigureModule(IServiceCollection services);
        void ConfigureModulePipeline(IApplicationBuilder appBuilder);
    }
}
