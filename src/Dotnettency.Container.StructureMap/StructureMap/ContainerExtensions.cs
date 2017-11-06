/// Taken from https://github.com/structuremap/StructureMap.Microsoft.DependencyInjection/
/// Licenced under MIT Licence.
/// With changes by Darrell Tunnell.
/// 
using Dotnettency.Container.StructureMap.StructureMap;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dotnettency.Container.StructureMap
{
    public static partial class ContainerExtensions
    {
        /// <summary>
        /// Populates the container using the specified service descriptors.
        /// </summary>
        /// <remarks>
        /// This method should only be called once per container.
        /// </remarks>
        /// <param name="container">The container.</param>
        /// <param name="descriptors">The service descriptors.</param>
        public static void Populate(this IContainer container, IEnumerable<ServiceDescriptor> descriptors)
        {
            container.Configure(config => config.Populate(descriptors));
        }
        
        /// <summary>
        /// Populates the container using the specified service descriptors.
        /// </summary>
        /// <remarks>
        /// This method should only be called once per container.
        /// </remarks>
        /// <param name="config">The configuration.</param>
        /// <param name="descriptors">The service descriptors.</param>
        public static void Populate(this ConfigurationExpression config, IEnumerable<ServiceDescriptor> descriptors)
        {
            Populate((Registry)config, descriptors);
        }
        
        /// <summary>
        /// Populates the registry using the specified service descriptors.
        /// </summary>
        /// <remarks>
        /// This method should only be called once per container.
        /// </remarks>
        /// <param name="registry">The registry.</param>
        /// <param name="descriptors">The service descriptors.</param>
        public static void Populate(this Registry registry, IEnumerable<ServiceDescriptor> descriptors)
        {
            registry.Policies.ConstructorSelector<AspNetConstructorSelector>();

            registry.For<ITenantContainerAdaptor>()
                .LifecycleIs(Lifecycles.Container)
                .Use<StructureMapTenantContainerAdaptor>();

            registry.Forward<ITenantContainerAdaptor, IServiceProvider>();        

            registry.For<IServiceScopeFactory>()
                .LifecycleIs(Lifecycles.Container)
                .Use<TenantContainerServiceScopeFactory>();

            registry.Register(descriptors);
        }
        
        private static void Register(this IProfileRegistry registry, IEnumerable<ServiceDescriptor> descriptors)
        {
            foreach (var descriptor in descriptors)
            {
                registry.Register(descriptor);
            }
        }

        private static void Register(this IProfileRegistry registry, ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
            {
                registry.For(descriptor.ServiceType)
                    .LifecycleIs(descriptor.Lifetime)
                    .Use(descriptor.ImplementationType);
                return;
            }

            if (descriptor.ImplementationFactory != null)
            {
                registry.For(descriptor.ServiceType)
                    .LifecycleIs(descriptor.Lifetime)
                    .Use(descriptor.CreateFactory());
                return;
            }

            registry.For(descriptor.ServiceType)
                .LifecycleIs(descriptor.Lifetime)
                .Use(descriptor.ImplementationInstance);
        }
        
        private static Expression<Func<IContext, object>> CreateFactory(this ServiceDescriptor descriptor)
        {
            return context => descriptor.ImplementationFactory(context.GetInstance<IServiceProvider>());
        }
    }
}
