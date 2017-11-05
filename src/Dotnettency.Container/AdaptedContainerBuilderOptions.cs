using System;

namespace Dotnettency.Container
{
    public class AdaptedContainerBuilderOptions<TTenant>
        where TTenant : class
    {
        public ContainerBuilderOptions<TTenant> ContainerBuilderOptions { get; set; }
        public Func<ITenantContainerAdaptor> HostContainerAdaptorFactory { get; set; }

        public AdaptedContainerBuilderOptions(ContainerBuilderOptions<TTenant> parentOptions, Func<ITenantContainerAdaptor> adaptorFactory)
        {
            ContainerBuilderOptions = parentOptions;
            HostContainerAdaptorFactory = adaptorFactory;

            ContainerBuilderOptions.Builder.ServiceProviderFactory = new Func<IServiceProvider>(() =>
            {
                return HostContainerAdaptorFactory();
            });
        }
    }
}