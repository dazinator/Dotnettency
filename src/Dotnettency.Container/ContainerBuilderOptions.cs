using Microsoft.Extensions.DependencyInjection;

namespace WebExperiment
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