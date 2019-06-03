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
            builder.Services.AddScoped<ITenantRequestContainerAccessor<TTenant>, TenantRequestContainerAccessor<TTenant>>();            

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

        public IServiceCollection DefaultServices { get; set; }

        /// <summary>
        /// Services that will be added to the tenants IServiceCollection by default. Sometimes when an IServiceCollection.AddXYZ() method is called,
        /// the particular implementation may depend upon other services being already registered in the IServiceCollection for it to behave correctly.
        /// You can specify these default services here, and they will be added into the tenants IServiceCollection, prior to it then being passed into the tenants Configure() method.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public ContainerBuilderOptions<TTenant> SetDefaultServices(IServiceCollection services)
        {
            DefaultServices = services;
            return this;
        }





    }
}
