using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Container
{

    public sealed class TenantContainerServiceScopeFactory : IServiceScopeFactory
    {
        private readonly ITenantContainerAdaptor _container;

        public TenantContainerServiceScopeFactory(ITenantContainerAdaptor container)
        {
            _container = container;
        }

        public IServiceScope CreateScope()
        {
            return new TenantContainerServiceScope(_container.CreateScope(_container.ContainerName + " - Scoped()"));
        }

        private class TenantContainerServiceScope : IServiceScope
        {
            private readonly ITenantContainerAdaptor _container;

            public TenantContainerServiceScope(ITenantContainerAdaptor container)
            {
                _container = container;
                ServiceProvider = _container;
            }

            public IServiceProvider ServiceProvider { get; private set; }

            public void Dispose()
            {
                _container.Dispose();
            }
        }
    }

}
