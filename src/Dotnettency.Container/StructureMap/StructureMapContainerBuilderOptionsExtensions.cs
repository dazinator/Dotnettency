using StructureMap;
using System;

namespace WebExperiment
{

    public static class StructureMapContainerBuilderOptionsExtensions
    //  where TTenant : class
    {


        public static IServiceProvider ConfigureStructureMapContainer<TTenant>(this ContainerBuilderOptions<TTenant> options,
          Action<TTenant, ConfigurationExpression> configureTenant)
          where TTenant : class
        {
            var container = new Container();
            container.Populate(options.Services);

            // var tenantContainer = container.CreateChildContainer();

            container.Configure(_ =>
                _.For<ITenantContainerBuilder<TTenant>>()
                    .Use(new StructureMapTenantContainerBuilder<TTenant>(container, configureTenant))
            );

            // now configure nested container per tenant.
            return container.GetInstance<IServiceProvider>();
            // var containerBuilderOptions = new ContainerBuilderOptions<TTenant>();
            // return options(containerBuilderOptions);
        }

        public static IServiceProvider ConfigureStructureMapContainer<TTenant>(this ContainerBuilderOptions<TTenant> options,
            Action<ConfigurationExpression> configureTenant)
            where TTenant : class
        {
            var container = new Container();
            container.Populate(options.Services);

            // var tenantContainer = container.CreateChildContainer();

            container.Configure(_ =>
              _.For<ITenantContainerBuilder<TTenant>>()
                  .Use(new StructureMapTenantContainerBuilder<TTenant>(container, (tenant, config) => configureTenant(config)))
          );

            // now configure nested container per tenant.
            return container.GetInstance<IServiceProvider>();
            // var containerBuilderOptions = new ContainerBuilderOptions<TTenant>();
            // return options(containerBuilderOptions);
        }
    }


}