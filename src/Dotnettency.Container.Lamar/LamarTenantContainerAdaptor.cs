using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Dotnettency.Container
{

    public class LamarTenantContainerAdaptor : ITenantContainerAdaptor
    {
        private readonly IContainer _container;
        private readonly Guid _id;
        private readonly ILogger<LamarTenantContainerAdaptor> _logger;

        public LamarTenantContainerAdaptor(
            ILogger<LamarTenantContainerAdaptor> logger,
            IContainer container,
            ContainerRole role = ContainerRole.Root,
            string name = "") : base()
        {


            _logger = logger;
            _container = container;
            _id = Guid.NewGuid();
            Role = role;

            if (name == null)
            {
                ContainerName = "Lamar";
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
                _logger.LogDebug("Container Created: {id}, {role}", _id, ContainerName, Role);
            }
        }

        public ContainerRole Role { get; set; }
        public string ContainerName { get; set; }
        public Guid ContainerId => _id;

        public void Configure(Action<IServiceCollection> configure)
        {
            _container.Configure(_ =>
            {
                _logger.LogDebug("Configuring container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
                configure(_);
                _logger.LogDebug("Root Container Adaptor Created: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            });
        }

        public ITenantContainerAdaptor CreateNestedContainer(string Name)
        {
            _logger.LogDebug("Creating nested container from container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            return new LamarNestedTenantContainerAdpator(_logger, _container.GetNestedContainer(), Name);
        }

        public ITenantContainerAdaptor CreateChildContainer(string Name)
        {
            _logger.LogCritical("Lamar doesn'ts support creating child containers: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            throw new Exception("Lamar doesn't support creating child containers.");
         //   return new LamarTenantContainerAdaptor(_logger, _container.CreateChildContainer(), ContainerRole.Child, Name);
        }

        public void Dispose()
        {
            _logger.LogDebug("Disposing of container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            _container.Dispose();
        }

        public ITenantContainerAdaptor CreateChildContainerAndConfigure(string Name, Action<IServiceCollection> configure)
        {
            ITenantContainerAdaptor container = CreateChildContainer(Name);
            Configure(configure);
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

        public object GetService(Type serviceType)
        {
            return _container.GetService(serviceType);
        }
    }
}
