using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{


    public class AutofacTenantContainerAdaptor : AutofacServiceProvider, ITenantContainerAdaptor
    {
        /// <summary>
        /// Marker object-tag for the tenant-level lifetime scope.
        /// </summary>
        internal static readonly object TenantLifetimeScopeTag = "tenantLifetime";

        private readonly ILifetimeScope _container;
        private readonly Guid _id;
        private readonly ILogger<AutofacTenantContainerAdaptor> _logger;

        public AutofacTenantContainerAdaptor(
            ILogger<AutofacTenantContainerAdaptor> logger,
            ILifetimeScope container,
            ContainerRole role = ContainerRole.Root,
            string name = "") : base(container)
        {
            _logger = logger;
            _container = container;
            _id = Guid.NewGuid();
            Role = role;

            if (name == null)
            {
                ContainerName = _container.Tag?.ToString() ?? "NULL";
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
                _logger.LogDebug("Container Created: {id}", _id, ContainerName);
            }
        }

        public ContainerRole Role { get; set; }
        public string ContainerName { get; set; }
        public Guid ContainerId => _id;

        public void Configure(Action<IServiceCollection> configure)
        {

            _logger.LogDebug("Configuring container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            ServiceCollection services = new ServiceCollection();
            configure(services);

            ContainerBuilder builder = new ContainerBuilder();
            builder.Populate(services);            
            builder.Update(_container.ComponentRegistry);

            _logger.LogDebug("Configured container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
        }

        public ITenantContainerAdaptor CreateNestedContainer(string Name)
        {

           // return new AutofacServiceScope(this._lifetimeScope.BeginLifetimeScope());
            return CreateNestedContainerAndConfigure(Name, null);
           // throw new NotImplementedException();
            //_logger.LogDebug("Creating nested container from container: {id}, {containerNAme}, {role}", _id, ContainerName, _container.Role);
            //ILifetimeScope perRequestScope = _container.BeginLifetimeScope()


            //return new AutofacTenantContainerAdaptor(_logger, _container.CHI(), ContainerRole.Scoped, Name);
        }

        public ITenantContainerAdaptor CreateChildContainer(string Name)
        {
            throw new NotImplementedException();
            //  _logger.LogDebug("Creating child container from container: {id}, {containerNAme}, {role}", _id, ContainerName, _container.Role);
            // return new AutofacTenantContainerAdaptor(_logger, _container.CreateChildContainer(), ContainerRole.Child, Name);
        }

        public new void Dispose()
        {
            _logger.LogDebug("Disposing of container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
           // _container.Dispose();
            base.Dispose();
        }

        public ITenantContainerAdaptor CreateChildContainerAndConfigure(string Name, Action<IServiceCollection> configure)
        {
            var scope = _container.BeginLifetimeScope(TenantLifetimeScopeTag, (builder) =>
            {
                ServiceCollection services = new ServiceCollection();
                configure(services);
                builder.Populate(services);
            });

             _logger.LogDebug("Creating child container from container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
             return new AutofacTenantContainerAdaptor(_logger, scope, ContainerRole.Child, Name);
        }

        public async Task<ITenantContainerAdaptor> CreateChildContainerAndConfigureAsync(string Name, Func<IServiceCollection, Task> configure)
        {

            ServiceCollection services = new ServiceCollection();
            await configure(services);

            var scope = _container.BeginLifetimeScope(TenantLifetimeScopeTag, (builder) =>
            {
                builder.Populate(services);
            });

            _logger.LogDebug("Creating child container from container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            return new AutofacTenantContainerAdaptor(_logger, scope, ContainerRole.Child, Name);
        }

        public ITenantContainerAdaptor CreateNestedContainerAndConfigure(string Name, Action<IServiceCollection> configure)
        {            
            var scope = _container.BeginLifetimeScope((builder) =>
            {
                ServiceCollection services = new ServiceCollection();
                configure?.Invoke(services);
                builder.Populate(services);
            });

            _logger.LogDebug("Creating nested container from container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
            return new AutofacTenantContainerAdaptor(_logger, scope, ContainerRole.Scoped, Name);
        }

        public void AddServices(Action<IServiceCollection> configure)
        {
            Configure(configure);
        }
    }
}
