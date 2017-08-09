using StructureMap;
using System;
using Microsoft.Extensions.DependencyInjection;
using Dotnettency.Container.StructureMap;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class StructureMapTenantContainerAdaptor : ITenantContainerAdaptor
    {
        private readonly IContainer _container;
        private readonly Guid _id;

        public StructureMapTenantContainerAdaptor(IContainer container, ContainerRole role)
        {
            _container = container;
            _id = Guid.NewGuid();
            Role = role;
            //ServiceProvider = new Lazy<IServiceProvider>(() =>
            //{
            //    return 
            //});
        }
        public IServiceProvider GetServiceProvider()
        {
            return _container.GetInstance<IServiceProvider>();
        }


        public ContainerRole Role { get; set; }

        public string ContainerName => _container.Name;

        public Guid ContainerId => _id;

        public void Configure(Action<IServiceCollection> configure)
        {
            //return Task.Run(() =>
            //{
            _container.Configure(_ =>
            {
                var services = new ServiceCollection();
                configure(services);
                _.Populate(services);
            });
            // });
        }

        public ITenantContainerAdaptor CreateNestedContainer()
        {
            return new StructureMapTenantContainerAdaptor(_container.GetNestedContainer(), ContainerRole.Scoped);
        }

        public ITenantContainerAdaptor CreateChildContainer()
        {
            return new StructureMapTenantContainerAdaptor(_container.CreateChildContainer(), ContainerRole.Child);
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}