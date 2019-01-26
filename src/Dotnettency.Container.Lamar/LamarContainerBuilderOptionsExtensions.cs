using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public static class LamarContainerBuilderOptionsExtensions
    {
        public static AdaptedContainerBuilderOptions<TTenant> WithLamar<TTenant>(
            this ContainerBuilderOptions<TTenant> options,
            Action<TTenant, IServiceCollection> configureTenant)
            where TTenant : class
        {
            Func<ITenantContainerAdaptor> adaptorFactory = new Func<ITenantContainerAdaptor>(() =>
            {
                // host level container.

                //registry.For<ITenantContainerAdaptor>()
                //    .LifecycleIs(Lifecycles.Container)
                //    .Use<LamarTenantContainerAdaptor>();
                options.Builder.Services.AddScoped<ITenantContainerAdaptor, LamarTenantContainerAdaptor>();

                //registry.Forward<ITenantContainerAdaptor, IServiceProvider>();
                options.Builder.Services.AddScoped<IServiceProvider>((sp)=> {
                    return sp.GetRequiredService<ITenantContainerAdaptor>();
                });

                Lamar.Container container = new Lamar.Container(options.Builder.Services);
                
                //  container.Populate(options.Builder.Services);
                //registry.Policies.ConstructorSelector<AspNetConstructorSelector>();





                //registry.For<IServiceScopeFactory>()
                //    .LifecycleIs(Lifecycles.Container)
                //    .Use<TenantContainerServiceScopeFactory>();


                ITenantContainerAdaptor adaptedContainer = container.GetInstance<ITenantContainerAdaptor>();

                ITenantContainerEventsPublisher<TTenant> containerEventsPublisher = container.TryGetInstance<ITenantContainerEventsPublisher<TTenant>>();
                // add ITenantContainerBuilder<TTenant> service to the host container
                // This service can be used to build a child container (adaptor) for a particular tenant, when required.
                container.Configure(_ =>
                _.AddSingleton<ITenantContainerBuilder<TTenant>>(new TenantContainerBuilder<TTenant>(adaptedContainer, configureTenant, containerEventsPublisher)));

                ITenantContainerAdaptor adaptor = container.GetInstance<ITenantContainerAdaptor>();
                return adaptor;
            });

            AdaptedContainerBuilderOptions<TTenant> adapted = new AdaptedContainerBuilderOptions<TTenant>(options, adaptorFactory);

            return adapted;
        }
    }
}
