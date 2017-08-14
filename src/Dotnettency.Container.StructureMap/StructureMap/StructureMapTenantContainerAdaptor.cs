using StructureMap;
using System;
using Microsoft.Extensions.DependencyInjection;
using Dotnettency.Container.StructureMap;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Dotnettency.Container
{
    public class StructureMapTenantContainerAdaptor : StructureMapServiceProvider, ITenantContainerAdaptor
    {
        private readonly IContainer _container;
        private readonly Guid _id;
        private readonly ILogger<StructureMapTenantContainerAdaptor> _logger;

        public StructureMapTenantContainerAdaptor(ILogger<StructureMapTenantContainerAdaptor> logger, IContainer container, ContainerRole role = ContainerRole.Root) : base(container)
        {
            _logger = logger;
            _container = container;
            _id = Guid.NewGuid();
            Role = role;
            if (role == ContainerRole.Root)
            {
                  _logger.LogDebug("Root Container Adaptor Created: {id}, {containerNAme}, {role}", _id, _container.Name, _container.Role);
            }
            else
            {
                _logger.LogDebug("Container Created: {id}, {role}", _id, _container.Name, _container.Role);
            }
            //  _logger.LogInformation("Tenant Container Adaptor Created: {id}, {containerNAme}, {role}", _id, _container.Name, _container.Role);
            //ServiceProvider = new Lazy<IServiceProvider>(() =>
            //{
            //    return 
            //});
        }

        //private IServiceProvider GetServiceProvider()
        //{
        //    return _container.GetInstance<IServiceProvider>();
        //}

        public ContainerRole Role { get; set; }

        public string ContainerName => _container.Name;

        public Guid ContainerId => _id;

        public void Configure(Action<IServiceCollection> configure)
        {
            //return Task.Run(() =>
            //{
            _container.Configure(_ =>
            {
                _logger.LogDebug("Configuring container: {id}, {containerNAme}, {role}", _id, _container.Name, _container.Role);
                var services = new ServiceCollection();
                configure(services);
                _.Populate(services);
            });
            // });
        }

        public ITenantContainerAdaptor CreateNestedContainer()
        {
            _logger.LogDebug("Creating nested container from container: {id}, {containerNAme}, {role}", _id, _container.Name, _container.Role);
            return new StructureMapTenantContainerAdaptor(_logger, _container.GetNestedContainer(), ContainerRole.Scoped);
        }

        public ITenantContainerAdaptor CreateChildContainer()
        {
            _logger.LogDebug("Creating child container from container: {id}, {containerNAme}, {role}", _id, _container.Name, _container.Role);
            return new StructureMapTenantContainerAdaptor(_logger, _container.CreateChildContainer(), ContainerRole.Child);
        }

        public void Dispose()
        {
            _logger.LogDebug("Disposing of container: {id}, {containerNAme}, {role}", _id, _container.Name, _container.Role);
            _container.Dispose();
        }

        //public new object GetService(Type serviceType)
        //{
        //    var sp = GetServiceProvider();
        //    return sp.GetService(serviceType);
        //}
    }
}