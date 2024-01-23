using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Dazinator.Extensions.DependencyInjection.ChildContainers;
using Dotnettency.Container.Native;
using Microsoft.Extensions.Logging;

namespace Dotnettency
{

    public static class NativeContainerBuilderOptionsExtensions
    {
        
        public static AdaptedContainerBuilderOptions<TTenant> Native<TTenant>(
            this ContainerBuilderOptions<TTenant> options,
            Action<TenantShellItemBuilderContext<TTenant>, IServiceCollection> configureTenant)
            where TTenant : class
        {
           
            // We create a function that will be eventually called at startup, by IServiceProviderFactory set on HostBuilder. 
            Func<IServiceCollection, ITenantContainerAdaptor> adaptorFactory = new Func<IServiceCollection, ITenantContainerAdaptor>((s) =>
            {
                // Build the root container. This function must return this.
                // support resolving the container adaptor from inside the child container.
                // This lets consumers use this adapted interface to create child containers etc.
              
                var sp = s.BuildServiceProvider();
                
                var logger = sp.GetRequiredService<ILogger<NativeTenantContainerAdaptor>>();
                var rootContainerAdaptor = new NativeTenantContainerAdaptor(logger, sp, s, ContainerRole.Root, "Root");
                return rootContainerAdaptor;
            });
            
            // Register a builder service, that can be used build child containers.
            var applicationServices = options.Builder.Services;
            applicationServices.AddSingleton<ITenantContainerBuilder<TTenant>>(sp =>
            {
                var adaptedContainer = sp.GetRequiredService<ITenantContainerAdaptor>(); // this service must be resolvable by the inner sp. It is registered when the function below is called,
                // which is done when the IHost is configured to use our custom IServiceProviderFactory
                // We depend on this to build child containers.
                var containerEventsPublisher = sp.GetRequiredService<ITenantContainerEventsPublisher<TTenant>>();
                var tenantContainerBuilder = new DelegateActionTenantContainerBuilder<TTenant>(adaptedContainer, configureTenant, containerEventsPublisher);
                return tenantContainerBuilder;
            });
            

            AdaptedContainerBuilderOptions<TTenant> adapted = new AdaptedContainerBuilderOptions<TTenant>(options, adaptorFactory);
            return adapted;
        }

        public static AdaptedContainerBuilderOptions<TTenant> NativeAsync<TTenant>(
          this ContainerBuilderOptions<TTenant> options,
          Func<TenantShellItemBuilderContext<TTenant>, IChildServiceCollection, Task> configureTenant)
          where TTenant : class
        {
            // We create a function that will be eventually called at startup, by IServiceProviderFactory set on HostBuilder. 
            Func<IServiceCollection, ITenantContainerAdaptor> adaptorFactory = new Func<IServiceCollection, ITenantContainerAdaptor>((s) =>
            {
                var sp = s.BuildServiceProvider();
                var logger = sp.GetRequiredService<ILogger<NativeTenantContainerAdaptor>>();
                var rootContainerAdaptor = new NativeTenantContainerAdaptor(logger, sp, s, ContainerRole.Root, "Root");
                return rootContainerAdaptor;
            });
            
            // Register a builder service, that can be used build child containers.
            var applicationServices = options.Builder.Services;
            applicationServices.AddSingleton<ITenantContainerBuilder<TTenant>>(sp =>
            {
                var adaptedContainer = sp.GetRequiredService<ITenantContainerAdaptor>(); // this service must be registered by the container adaptor. It is registered when the function below is called,
                var containerEventsPublisher = sp.GetRequiredService<ITenantContainerEventsPublisher<TTenant>>();
                var tenantContainerBuilder = new DelegateTaskTenantContainerBuilder<TTenant>(adaptedContainer, configureTenant, containerEventsPublisher);
                return tenantContainerBuilder;
            });

            AdaptedContainerBuilderOptions<TTenant> adapted = new AdaptedContainerBuilderOptions<TTenant>(options, adaptorFactory);
            return adapted;
        }

    }
}
