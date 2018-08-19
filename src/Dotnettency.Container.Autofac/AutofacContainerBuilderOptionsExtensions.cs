using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class AutofacContainerBuilderOptionsExtensions
    {
        public static AdaptedContainerBuilderOptions<TTenant> WithAutofac<TTenant>(
            this ContainerBuilderOptions<TTenant> options,
            Action<TTenant, IServiceCollection> configureTenant)
            where TTenant : class
        {
            var adaptorFactory = new Func<ITenantContainerAdaptor>(() =>
            {
                // host level container.
                var builder = new ContainerBuilder();
                builder.Populate(options.Builder.Services);

                var container = builder.Build();
                var adaptedContainer = container.Resolve<ITenantContainerAdaptor>();

                ITenantContainerEventsPublisher<TTenant> containerEventsPublisher;
                //  bool containerEventsPublisher =
                container.TryResolve<ITenantContainerEventsPublisher<TTenant>>(out containerEventsPublisher);
                // add ITenantContainerBuilder<TTenant> service to the host container
                // This service can be used to build a child container (adaptor) for a particular tenant, when required.

                var updateBuilder = new ContainerBuilder();
                updateBuilder.RegisterInstance(new TenantContainerBuilder<TTenant>(adaptedContainer, configureTenant, containerEventsPublisher)).As<ITenantContainerBuilder<TTenant>>();
                updateBuilder.Update(container);


                //container.Configure(_ =>
                //    _.For<ITenantContainerBuilder<TTenant>>()
                //        .Use()
                //    );

                var adaptor = container.Resolve<ITenantContainerAdaptor>();              
                return adaptor;
            });

            var adapted = new AdaptedContainerBuilderOptions<TTenant>(options, adaptorFactory);
            return adapted;
        }
    }
}
