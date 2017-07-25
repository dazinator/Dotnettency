using StructureMap;
using System;

namespace Dotnettency.Container
{
    public class StructureMapTenantContainerAdaptor : ITenantContainerAdaptor
    {
        private readonly IContainer _container;
        private readonly Guid _id;

        public StructureMapTenantContainerAdaptor(IContainer container)
        {
            _container = container;
            _id = Guid.NewGuid();
            ServiceProvider = new Lazy<IServiceProvider>(() =>
            {
                return _container.GetInstance<IServiceProvider>();
            });
        }
        public Lazy<IServiceProvider> ServiceProvider { get; }

        public string ContainerName => _container.Name;

        public Guid ContainerId => _id;

        public ITenantContainerAdaptor CreateNestedContainer()
        {
            return new StructureMapTenantContainerAdaptor(_container.GetNestedContainer());
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}