using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using System;

namespace Dotnettency.Container.StructureMap
{
    public sealed class StructureMapServiceScopeFactory : IServiceScopeFactory
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

            public StructureMapServiceScope(IContainer container)
            {
                _container = container;
                ServiceProvider = container.GetInstance<IServiceProvider>();
            }

            public IServiceProvider ServiceProvider { get; private set; }

            public void Dispose()
            {
                _container.Dispose();
            }
        }
    }
}

