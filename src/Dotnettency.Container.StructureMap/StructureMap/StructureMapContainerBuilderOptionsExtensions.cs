using Dotnettency.Container;
using Dotnettency.Container.StructureMap;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class StructureMapContainerBuilderOptionsExtensions
    {
        public static AdaptedContainerBuilderOptions<TTenant> WithStructureMap<TTenant>(this ContainerBuilderOptions<TTenant> options,
            Action<TTenant, IServiceCollection> configureTenant)
            where TTenant : class
        {
            var adaptorFactory = new Func<ITenantContainerAdaptor>(() =>
            {
                // host level container.
                var container = new StructureMap.Container();
                container.Populate(options.Builder.Services);
                var adaptedContainer = container.GetInstance<ITenantContainerAdaptor>();
                // add ITenantContainerBuilder<TTenant> service to the host container
                // This service can be used to build a child container (adaptor) for a particular tenant, when required.
                container.Configure(_ =>
                    _.For<ITenantContainerBuilder<TTenant>>()
                        .Use(new TenantContainerBuilder<TTenant>(adaptedContainer, configureTenant))
                    );

                var adaptor = container.GetInstance<ITenantContainerAdaptor>();
                return adaptor;
            });

            var adapted = new AdaptedContainerBuilderOptions<TTenant>(options, adaptorFactory);
            
            return adapted;
        }
    }
}
