using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.PerTenantHostingEnvironment
{
    public class TenantStartup : ITenantStartup
    {
        public TenantStartup()
        {
          
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //  var tenantService = new SomeTenantService() { Id = Guid.NewGuid() };
            //  services.AddSingleton<SomeTenantService>(tenantService);
        }
    }
}