using Dotnettency.Container.StructureMap;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public class StructureMapTenantContainerAdaptor : StructureMapServiceProvider, ITenantContainerAdaptor
    {
        private readonly IContainer _container;
        private readonly Guid _id;
        private readonly ILogger<StructureMapTenantContainerAdaptor> _logger;

        public StructureMapTenantContainerAdaptor(ILogger<StructureMapTenantContainerAdaptor> logger,
            IContainer container,
            ContainerRole role = ContainerRole.Root,
            string name = "") : base(container)
        {
            _logger = logger;
            _container = container;
            _id = Guid.NewGuid();
            Role = role;

            if (name == null)
            {
                ContainerName = _container.Name;
            }
            else
            {
                ContainerName = name;
            }

            if (role == ContainerRole.Root)
            {
                _logger.LogDebug("Root Container Created: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            }
            else
            {
                _logger.LogDebug("Container Created: {id}, {role}", _id, ContainerName, _container.Role);
            }
        }

        public ContainerRole Role { get; set; }
        public string ContainerName { get; set; }
        public Guid ContainerId => _id;

        public void Configure(Action<IServiceCollection> configure)
        {
            ServiceCollection services = new ServiceCollection();
            configure(services);
            Populate(services);            
        }

        public void Populate(IServiceCollection services)
        {
            _container.Configure(_ =>
            {
                _logger.LogDebug("Configuring container: {id}, {containerNAme}, {role}", _id, ContainerName, _container.Role);
                _.Populate(services);
                _logger.LogDebug("Root Container Adaptor Created: {id}, {containerNAme}, {role}", _id, ContainerName, _container.Role);
            });
        }

        public ITenantContainerAdaptor CreateNestedContainer(string Name)
        {
            _logger.LogDebug("Creating nested container from container: {id}, {containerNAme}, {role}", _id, ContainerName, _container.Role);
            return new StructureMapTenantContainerAdaptor(_logger, _container.GetNestedContainer(), ContainerRole.Scoped, Name);
        }

        public ITenantContainerAdaptor CreateChildContainer(string Name)
        {
            _logger.LogDebug("Creating child container from container: {id}, {containerNAme}, {role}", _id, ContainerName, _container.Role);
            return new StructureMapTenantContainerAdaptor(_logger, _container.CreateChildContainer(), ContainerRole.Child, Name);
        }

        public void Dispose()
        {
            _logger.LogDebug("Disposing of container: {id}, {containerNAme}, {role}", _id, ContainerName, _container.Role);
            _container.Dispose();
        }

        public ITenantContainerAdaptor CreateChildContainerAndConfigure(string Name, Action<IServiceCollection> configure)
        {
            ITenantContainerAdaptor container = CreateChildContainer(Name);
            Configure(configure); 
            return container;
        }

        public async Task<ITenantContainerAdaptor> CreateChildContainerAndConfigureAsync(string Name, Func<IServiceCollection, Task> configure)
        {
            ServiceCollection services = new ServiceCollection();
            await configure(services);
            Populate(services);
            ITenantContainerAdaptor container = CreateChildContainer(Name);           
            return container;         
        }

    public ITenantContainerAdaptor CreateNestedContainerAndConfigure(string Name, Action<IServiceCollection> configure)
    {
        ITenantContainerAdaptor container = CreateNestedContainer(Name);
        Configure(configure);
        return container;
    }

    public void AddServices(Action<IServiceCollection> configure)
    {
        Configure(configure);
    }
}
}
