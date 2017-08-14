using Dotnettency.Container;
using System;


namespace Dotnettency.Container
{
    public class AdaptedContainerBuilderOptions<TTenant>
         where TTenant : class
    {

        public AdaptedContainerBuilderOptions(ContainerBuilderOptions<TTenant> parentOptions, Func<ITenantContainerAdaptor> adaptorFactory)
        {
            ContainerBuilderOptions = parentOptions;
            HostContainerAdaptor = adaptorFactory;

            ContainerBuilderOptions.Builder.ServiceProviderFactory = new Func<IServiceProvider>(() =>
            {
                return HostContainerAdaptor();
            });
        }


        public ContainerBuilderOptions<TTenant> ContainerBuilderOptions { get; set; }

        public Func<ITenantContainerAdaptor> HostContainerAdaptor { get; set; }

    }
}