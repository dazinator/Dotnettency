using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using System;
using Dotnettency.Container.StructureMap;

namespace Dotnettency
{

    public static class StructureMapContainerBuilderOptionsExtensions
    {
        //public static ContainerBuilderOptions<TTenant> WithStructureMap<TTenant>(this ContainerBuilderOptions<TTenant> options,
        //  Action<TTenant, ConfigurationExpression> configureTenant)
        //  where TTenant : class
        //{


        //    // Set a func, that is only invoked when the ServiceProvider is required. This ensures it runs after all other configuration methods
        //    // which is important as other methods can still add new services to the servicecollection after this one is invoked and we
        //    // dont want them to be missed when populating the container.
        //    options.Builder.BuildServiceProvider = new Func<IServiceProvider>(() =>
        //    {
        //        var container = new StructureMap.Container();
        //        container.Populate(options.Builder.Services);
        //        container.Configure(_ =>
        //            _.For<ITenantContainerBuilder<TTenant>>()
        //                .Use(new StructureMapTenantContainerBuilder<TTenant>(container, configureTenant))
        //        );
        //        return container.GetInstance<IServiceProvider>();
        //    });

        //    return options;
        //}

        //public static AdaptedContainerBuilderOptions<TTenant> WithStructureMap<TTenant>(this ContainerBuilderOptions<TTenant> options,
        //    Action<ConfigurationExpression> configureTenant)
        //    where TTenant : class
        //{

        //    // Set a func, that is only invoked when the ServiceProvider is required. This ensures it runs after all other configuration methods
        //    // which is important as other methods can still add new services to the servicecollection after this one is invoked and we
        //    // dont want them to be missed when populating the container.
        //    options.Builder.BuildServiceProvider = new Func<IServiceProvider>(() =>
        //    {
        //        var container = new StructureMap.Container();
        //        container.Populate(options.Builder.Services);

        //        container.Configure(_ =>
        //          _.For<ITenantContainerBuilder<TTenant>>()
        //              .Use(new StructureMapTenantContainerBuilder<TTenant>(container, (tenant, config) => configureTenant(config)))
        //      );
        //        return container.GetInstance<IServiceProvider>();
        //    });

        //    return options;

        //}

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

                // new StructureMap.Pipeline.ExplicitArguments("role", ContainerRole.Root)
                var adaptor = container.GetInstance<ITenantContainerAdaptor>();
                // new StructureMapTenantContainerAdaptor(container, ContainerRole.Root);
                return adaptor;
            });

            var adapted = new AdaptedContainerBuilderOptions<TTenant>(options, adaptorFactory);

            // Set a func, that is only invoked when the ServiceProvider is required. This ensures it runs after all other configuration methods
            // which is important as other methods can still add new services to the servicecollection after this one is invoked and we
            // dont want them to be missed when populating the container.


            //options.Builder.BuildServiceProvider = new Func<IServiceProvider>(() =>
            //{

            //    // now configure nested container per tenant.
            //    return container.GetInstance<IServiceProvider>();
            //});

            return adapted;
        }

    }
}