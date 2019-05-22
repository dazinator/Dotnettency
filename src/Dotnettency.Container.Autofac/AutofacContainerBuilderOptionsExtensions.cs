using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class AutofacContainerBuilderOptionsExtensions
    {
        public static AdaptedContainerBuilderOptions<TTenant> Autofac<TTenant>(
            this ContainerBuilderOptions<TTenant> options,
            Action<TTenant, IServiceCollection> configureTenant)
            where TTenant : class
        {
            Func<ITenantContainerAdaptor> adaptorFactory = new Func<ITenantContainerAdaptor>(() =>
            {
                // host level container.
                ContainerBuilder builder = new ContainerBuilder();
                builder.Populate(options.Builder.Services);
                builder.AddDotnettencyContainerServices();
                
                

                IContainer container = builder.Build();
                ITenantContainerAdaptor adaptedContainer = container.Resolve<ITenantContainerAdaptor>();

                //  bool containerEventsPublisher =
                container.TryResolve<ITenantContainerEventsPublisher<TTenant>>(out ITenantContainerEventsPublisher<TTenant> containerEventsPublisher);
                // add ITenantContainerBuilder<TTenant> service to the host container
                // This service can be used to build a child container (adaptor) for a particular tenant, when required.

                ContainerBuilder updateBuilder = new ContainerBuilder();
                updateBuilder.RegisterInstance(new TenantContainerBuilder<TTenant>(adaptedContainer, configureTenant, containerEventsPublisher)).As<ITenantContainerBuilder<TTenant>>();
                updateBuilder.Update(container);


                //container.Configure(_ =>
                //    _.For<ITenantContainerBuilder<TTenant>>()
                //        .Use()
                //    );

                ITenantContainerAdaptor adaptor = container.Resolve<ITenantContainerAdaptor>();
                return adaptor;
            });

            AdaptedContainerBuilderOptions<TTenant> adapted = new AdaptedContainerBuilderOptions<TTenant>(options, adaptorFactory);
            return adapted;
        }
    }
}
