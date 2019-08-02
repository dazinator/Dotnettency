using Dotnettency.Container;
using Dotnettency.Container.StructureMap;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class StructureMapContainerBuilderOptionsExtensions
    {
        public static AdaptedContainerBuilderOptions<TTenant> WithStructureMap<TTenant>(
            this ContainerBuilderOptions<TTenant> options,
            Action<TTenant, IServiceCollection> configureTenant)
            where TTenant : class
        {
            var adaptorFactory = new Func<ITenantContainerAdaptor>(() =>
            {
                // host level container.
                var container = new StructureMap.Container();
                container.Populate(options.Builder.Services);
                var adaptedContainer = container.GetInstance<ITenantContainerAdaptor>();

                var containerEventsPublisher = container.TryGetInstance<ITenantContainerEventsPublisher<TTenant>>();
                // add ITenantContainerBuilder<TTenant> service to the host container
                // This service can be used to build a child container (adaptor) for a particular tenant, when required.
                var defaultServices = options.DefaultServices;
                container.Configure(_ =>
                    _.For<ITenantContainerBuilder<TTenant>>()
                        .Use(new DelegateActionTenantContainerBuilder<TTenant>(defaultServices, adaptedContainer, configureTenant, containerEventsPublisher))
                    );

                // NOTE: Im not sure why I was resolving ITenantContainerAdaptor twice, changed to just return previous instance.
                //var adaptor = container.GetInstance<ITenantContainerAdaptor>();
                //return adaptor;
                return adaptedContainer;
            });

            var adapted = new AdaptedContainerBuilderOptions<TTenant>(options, adaptorFactory);

            return adapted;
        }
    }
}
