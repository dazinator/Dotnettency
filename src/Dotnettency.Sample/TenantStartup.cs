using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Container
{
    public class TenantStartup : ITenantStartup
    {

      //  private readonly IServiceProvider _sp;
        public TenantStartup(IServiceProvider sp)
        {
       //     _sp = sp;
        }

        public void ConfigureServices(IServiceCollection services)
        {
          //  var tenantService = new SomeTenantService() { Id = Guid.NewGuid() };
          //  services.AddSingleton<SomeTenantService>(tenantService);

           
        }
    }
}
