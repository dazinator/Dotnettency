using StructureMap;
using System;
using Microsoft.Extensions.DependencyInjection;
using Dotnettency.Container.StructureMap;

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
            //ServiceProvider = new Lazy<IServiceProvider>(() =>
            //{
            //    return 
            //});
        }
        public IServiceProvider GetServiceProvider()
        {
            return _container.GetInstance<IServiceProvider>();
        }
      

        public string ContainerName => _container.Name;

        public Guid ContainerId => _id;

        public void Configure(Action<IServiceCollection> configure)
        {
            _container.Configure(_ =>
            {
                var services = new ServiceCollection();
                configure(services);
                _.Populate(services);
            });
        }

        public ITenantContainerAdaptor CreateNestedContainer()
        {
            return new StructureMapTenantContainerAdaptor(_container.GetNestedContainer());
        }

        public ITenantContainerAdaptor CreateChildContainer()
        {
            return new StructureMapTenantContainerAdaptor(_container.CreateChildContainer());
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}