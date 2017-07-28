using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;


namespace Dotnettency
{
    public static class StructureMapContainerBuilderOptionsExtensions
    {
        public static ContainerBuilderOptions<TTenant> WithStructureMap<TTenant>(this ContainerBuilderOptions<TTenant> options,
          Action<TTenant, ConfigurationExpression> configureTenant)
          where TTenant : class
        {


            // Set a func, that is only invoked when the ServiceProvider is required. This ensures it runs after all other configuration methods
            // which is important as other methods can still add new services to the servicecollection after this one is invoked and we
            // dont want them to be missed when populating the container.
            options.Builder.BuildServiceProvider = new Func<IServiceProvider>(() =>
            {
                var container = new StructureMap.Container();
                container.Populate(options.Builder.Services);
                container.Configure(_ =>
                    _.For<ITenantContainerBuilder<TTenant>>()
                        .Use(new StructureMapTenantContainerBuilder<TTenant>(container, configureTenant))
                );
                return container.GetInstance<IServiceProvider>();
            });

            return options;
        }

        public static ContainerBuilderOptions<TTenant> WithStructureMap<TTenant>(this ContainerBuilderOptions<TTenant> options,
            Action<ConfigurationExpression> configureTenant)
            where TTenant : class
        {

            // Set a func, that is only invoked when the ServiceProvider is required. This ensures it runs after all other configuration methods
            // which is important as other methods can still add new services to the servicecollection after this one is invoked and we
            // dont want them to be missed when populating the container.
            options.Builder.BuildServiceProvider = new Func<IServiceProvider>(() =>
            {
                var container = new StructureMap.Container();
                container.Populate(options.Builder.Services);

                container.Configure(_ =>
                  _.For<ITenantContainerBuilder<TTenant>>()
                      .Use(new StructureMapTenantContainerBuilder<TTenant>(container, (tenant, config) => configureTenant(config)))
              );
                return container.GetInstance<IServiceProvider>();
            });

            return options;

        }

        public static ContainerBuilderOptions<TTenant> WithStructureMapServiceCollection<TTenant>(this ContainerBuilderOptions<TTenant> options,
        Action<TTenant, IServiceCollection> configureTenant)
        where TTenant : class
        {

            // Set a func, that is only invoked when the ServiceProvider is required. This ensures it runs after all other configuration methods
            // which is important as other methods can still add new services to the servicecollection after this one is invoked and we
            // dont want them to be missed when populating the container.
            options.Builder.BuildServiceProvider = new Func<IServiceProvider>(() =>
            {
                var container = new StructureMap.Container();
                container.Populate(options.Builder.Services);

                container.Configure(_ =>
                    _.For<ITenantContainerBuilder<TTenant>>()
                        .Use(new StructureMapTenantContainerBuilder<TTenant>(container, (tenant, configurationExpression) =>
                        {
                            var tenantServices = new ServiceCollection();
                            configureTenant(tenant, tenantServices);
                            configurationExpression.Populate(tenantServices);
                        }))
                    );

                // now configure nested container per tenant.
                return container.GetInstance<IServiceProvider>();
            });

            return options;
        }

    }


}