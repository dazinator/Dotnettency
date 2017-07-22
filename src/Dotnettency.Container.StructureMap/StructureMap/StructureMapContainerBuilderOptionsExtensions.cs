using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;


namespace Dotnettency
{
    public static class StructureMapContainerBuilderOptionsExtensions
    {
        public static ContainerBuilderOptions<TTenant> ConfigureStructureMapContainer<TTenant>(this ContainerBuilderOptions<TTenant> options,
          Action<TTenant, ConfigurationExpression> configureTenant)
          where TTenant : class
        {
            var container = new StructureMap.Container();
            container.Populate(options.Builder.Services);

            container.Configure(_ =>
                _.For<ITenantContainerBuilder<TTenant>>()
                    .Use(new StructureMapTenantContainerBuilder<TTenant>(container, configureTenant))
            );

            // now configure nested container per tenant.
            options.Builder.ServiceProvider = container.GetInstance<IServiceProvider>();
            return options;
        }

        public static ContainerBuilderOptions<TTenant> ConfigureStructureMapContainer<TTenant>(this ContainerBuilderOptions<TTenant> options,
            Action<ConfigurationExpression> configureTenant)
            where TTenant : class
        {
            var container = new StructureMap.Container();
            container.Populate(options.Builder.Services);

            container.Configure(_ =>
              _.For<ITenantContainerBuilder<TTenant>>()
                  .Use(new StructureMapTenantContainerBuilder<TTenant>(container, (tenant, config) => configureTenant(config)))
          );

            // now configure nested container per tenant.
            options.Builder.ServiceProvider = container.GetInstance<IServiceProvider>();
            return options;
        }
    }


}