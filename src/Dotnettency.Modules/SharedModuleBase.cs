using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Modules
{
    public abstract class SharedModuleBase : ModuleBase, ISharedModule
    {
        public virtual void ConfigureMiddleware(IApplicationBuilder appBuilder)
        {
            // throw new NotImplementedException();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            // throw new NotImplementedException();
        }
    }
}
