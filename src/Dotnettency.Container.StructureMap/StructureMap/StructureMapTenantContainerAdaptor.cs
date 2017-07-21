using Dotnettency.Container;
using StructureMap;
using System;

namespace Dotnettency.Container
{

    public class StructureMapTenantContainerAdaptor : ITenantContainerAdaptor
    {
        private readonly IContainer _container;

        public StructureMapTenantContainerAdaptor(IContainer container)
        {
            _container = container;
            ServiceProvider = new Lazy<IServiceProvider>(() =>
            {
                return _container.GetInstance<IServiceProvider>();
            });
        }
        public Lazy<IServiceProvider> ServiceProvider { get; }

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
