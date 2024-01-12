using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnettency.Container
{
    public class AdaptedContainerBuilderOptions<TTenant>
        where TTenant : class
    {
        public AdaptedContainerBuilderOptions(ContainerBuilderOptions<TTenant> parentOptions, Func<IServiceCollection, ITenantContainerAdaptor> adaptorFactory)
        {
            ContainerBuilderOptions = parentOptions;
            HostContainerAdaptorFactory = adaptorFactory;

            ContainerBuilderOptions.Builder.ServiceProviderFactory = new Func<IServiceCollection, IServiceProvider>((s) =>
            {
                return HostContainerAdaptorFactory(s);
            });
        }

        public ContainerBuilderOptions<TTenant> ContainerBuilderOptions { get; set; }
        public Func<IServiceCollection, ITenantContainerAdaptor> HostContainerAdaptorFactory { get; set; }
    }
}