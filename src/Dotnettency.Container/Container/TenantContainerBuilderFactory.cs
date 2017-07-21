using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
     

    public class TenantContainerBuilderFactory<TTenant> : TenantContainerFactory<TTenant>
     where TTenant : class    
    {

        //  private readonly List<TTenantStartup> _startups;
        private readonly IServiceProvider _serviceProvider;
        // private readonly IServiceProvider _serviceProvider;

        //// private readonly  builder

        public TenantContainerBuilderFactory(IServiceProvider servicePrvider)
            : base(servicePrvider)
        {
            _serviceProvider = servicePrvider;
            //_startups = startups.ToList();
        }

        protected override async Task<ITenantContainerAdaptor> BuildContainer(TTenant currentTenant)
        {


            var builder = _serviceProvider.GetRequiredService<ITenantContainerBuilder<TTenant>>();
            var container = await builder.BuildAsync(currentTenant);
            //var startups = _serviceProvider.GetServices<TTenantStartup>();
            //var tenantServices = new ServiceCollection();
            //foreach (var item in startups)
            //{
            //    item.ConfigureServices(tenantServices);
            //}
            return container;
           // return tenantServices.BuildServiceProvider();
            //});
        }
    }


    //public class ConfigureTenantFromServicesTenantContainerFactory<TTenant> : ITenantContainerFactory<TTenant>
    //    where TTenant : class
    //{

    //    public ConfigureTenantFromServicesTenantContainerFactory()
    //    {

    //    }

    //    public Task<IServiceProvider> Get(TenantIdentifier identifier)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}




}
