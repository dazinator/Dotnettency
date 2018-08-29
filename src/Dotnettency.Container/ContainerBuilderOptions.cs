using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Container
{
    public class ContainerBuilderOptions<TTenant>
        where TTenant : class
    {
        
        public ContainerBuilderOptions(MultitenancyOptionsBuilder<TTenant> builder)
        {
            Builder = builder;
            ContainerEventsOptions = new TenantContainerEventsOptions<TTenant>(this);

            builder.Services.AddSingleton<ITenantContainerFactory<TTenant>, TenantContainerBuilderFactory<TTenant>>();
            builder.Services.AddScoped<ITenantContainerAccessor<TTenant>, TenantContainerAccessor<TTenant>>();

            builder.Services.AddSingleton<ITenantContainerEventsPublisher<TTenant>>((sp) =>
            {              
                var events = new TenantContainerEventsPublisher<TTenant>();

                var containerEventsOptions = this.ContainerEventsOptions;
                foreach (var item in containerEventsOptions.TenantContainerCreatedCallbacks)
                {
                    events.TenantContainerCreated += item;
                }

                foreach (var item in containerEventsOptions.NestedTenantContainerCreatedCallbacks)
                {
                    events.NestedTenantContainerCreated += item;
                }
                return events;
            });
            builder.Services.AddSingleton<ITenantContainerEvents<TTenant>>((sp) =>
            {              
                return sp.GetRequiredService<ITenantContainerEventsPublisher<TTenant>>();
            });
                
        }

        public MultitenancyOptionsBuilder<TTenant> Builder { get; set; }

        public TenantContainerEventsOptions<TTenant> ContainerEventsOptions { get; set; }

        public ContainerBuilderOptions<TTenant> Events(Action<TenantContainerEventsOptions<TTenant>> containerLifecycleOptions)
        {       
            containerLifecycleOptions?.Invoke(ContainerEventsOptions);
            return this;
        }

    }
}
