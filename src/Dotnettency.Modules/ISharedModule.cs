using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Modules
{
    /// <summary>
    /// An <see cref="ISharedModule"/> can configures services and middleware that can have a direct impact on all downstream <see cref="IModules"/>'s.
    /// It can register services into the container shared by all modules, and it can add middleware to the middleware pipeline that is invoked before module routing.
    /// </summary>
    public interface ISharedModule : IModule
    {
        void ConfigureMiddleware(IApplicationBuilder appBuilder);
        void ConfigureServices(IServiceCollection services);
    }
}
