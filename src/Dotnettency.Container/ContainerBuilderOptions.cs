using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Container
{
    //public class TenantServicesConfigurationProvider<TTenantStartup>
    //{
    //    public TenantServicesConfigurationProvider()
    //    {

    //    }
    //}



    public class ContainerBuilderOptions<TTenant>
    {
        public ContainerBuilderOptions(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; set; }

    }


}