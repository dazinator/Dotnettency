using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;

namespace Dotnettency.Container.StructureMap
{
    public static partial class ContainerExtensions
    {
        internal sealed class TenantContainerServiceScopeFactory : IServiceScopeFactory
        {
            public TenantContainerServiceScopeFactory(ITenantContainerAdaptor container)
            {
                Container = container; // new StructureMapTenantContainerAdaptor(container, ContainerRole.Root);
            }

            private ITenantContainerAdaptor Container { get; }

            public IServiceScope CreateScope()
            {
                return new TenantContainerServiceScope(Container.CreateNestedContainer());
            }


            private class TenantContainerServiceScope : IServiceScope
            {

                public TenantContainerServiceScope(ITenantContainerAdaptor container)
                {
                    Container = container;
                    ServiceProvider = Container;
                }

                private ITenantContainerAdaptor Container { get; }

                public IServiceProvider ServiceProvider { get; }

                public void Dispose() => Container.Dispose();

            }

        }

        internal sealed class StructureMapServiceScopeFactory : IServiceScopeFactory
        {
            public StructureMapServiceScopeFactory(IContainer container)
            {
                Container = container;
            }

            private IContainer Container { get; }

            public IServiceScope CreateScope()
            {
                return new StructureMapServiceScope(Container.GetNestedContainer());
            }


            private class StructureMapServiceScope : IServiceScope
            {

                public StructureMapServiceScope(IContainer container)
                {
                    Container = container;
                    ServiceProvider = container.GetInstance<IServiceProvider>();
                }

                private IContainer Container { get; }

                public IServiceProvider ServiceProvider { get; }

                public void Dispose() => Container.Dispose();

            }

        }
    }
}