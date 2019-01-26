using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Dotnettency.Container
{
    public class LamarNestedTenantContainerAdpator : ITenantContainerAdaptor
    {
        private readonly INestedContainer _container;
        private readonly Guid _id;
        private readonly ILogger<LamarNestedTenantContainerAdpator> _logger;

        public LamarNestedTenantContainerAdpator(
            ILogger<LamarNestedTenantContainerAdpator> logger,
            INestedContainer container,
            string name = "") : base()
        {
            _logger = logger;
            _container = container;
            _id = Guid.NewGuid();
            Role = ContainerRole.Scoped;

            if (name == null)
            {
                ContainerName = "Nested";
            }
            else
            {
                ContainerName = name;
            }

            _logger.LogDebug("Container Created: {id}, {role}", _id, ContainerName, Role);

        }

        public ContainerRole Role { get; set; }
        public string ContainerName { get; set; }
        public Guid ContainerId => _id;

        public void Configure(Action<IServiceCollection> configure)
        {
            _container.Configure(_ =>
            {
                _logger.LogDebug("Configuring container: {id}, {containerNAme}, {role}", _id, ContainerName, _container.Role);
                configure(_);
                _logger.LogDebug("Root Container Adaptor Created: {id}, {containerNAme}, {role}", _id, ContainerName, _container.Role);
            });
        }

        public ITenantContainerAdaptor CreateNestedContainer(string Name)
        {
            _logger.LogCritical("Lamar does not support creating a nested container from a nested container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            return this;
        }

        public ITenantContainerAdaptor CreateChildContainer(string Name)
        {
            _logger.LogCritical("Lamar does not support creating child containers from a nested container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            return this;
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
