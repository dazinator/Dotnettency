using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;

namespace Dotnettency.Container.StructureMap
{
    public static partial class ContainerExtensions
    {
        internal sealed class TenantContainerServiceScopeFactory : IServiceScopeFactory
        {
            private readonly ITenantContainerAdaptor _container;

            public TenantContainerServiceScopeFactory(ITenantContainerAdaptor container)
            {
                _container = container;
            }

            public IServiceScope CreateScope()
            {
                return new TenantContainerServiceScope(_container.CreateNestedContainer());
            }
            
            private class TenantContainerServiceScope : IServiceScope
            {
                private readonly ITenantContainerAdaptor _container;

                public IServiceProvider ServiceProvider { get; }

                public TenantContainerServiceScope(ITenantContainerAdaptor container)
                {
                    _container = container;
                    ServiceProvider = _container;
                }

                public void Dispose() => _container.Dispose();
            }
        }

        internal sealed class StructureMapServiceScopeFactory : IServiceScopeFactory
        {
            private readonly IContainer _container;

            public StructureMapServiceScopeFactory(IContainer container)
            {
                _container = container;
            }

            public IServiceScope CreateScope()
            {
                return new StructureMapServiceScope(_container.GetNestedContainer());
            }
            
            private class StructureMapServiceScope : IServiceScope
            {
                private readonly IContainer _container;

                public IServiceProvider ServiceProvider { get; }

                public StructureMapServiceScope(IContainer container)
                {
                    _container = container;
                    ServiceProvider = container.GetInstance<IServiceProvider>();
                }

                public void Dispose() => _container.Dispose();
            }
        }
    }
}
