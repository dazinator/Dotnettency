using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Container
{
    public interface ITenantStartup
    {
        void ConfigureServices(IServiceCollection services);
    }
}
