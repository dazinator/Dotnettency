using System;
using System.Linq;
using System.Threading.Tasks;
using Dazinator.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceCollection = Dazinator.Extensions.DependencyInjection.ServiceCollection;

namespace Dotnettency.Container.Native
{
    public class NativeTenantContainerAdaptor : ITenantContainerAdaptor
    {
        /// <summary>
        /// Marker object-tag for the tenant-level lifetime scope.
        /// </summary>
        internal static readonly object TenantLifetimeScopeTag = "tenantLifetime";

        private readonly Guid _id;
        private readonly ILogger<NativeTenantContainerAdaptor> _logger;
        private readonly IServiceProvider _innerServiceProvider;
        private readonly IServiceCollection _serviceCollection;
        private readonly IServiceScope _scope;

        public NativeTenantContainerAdaptor(
            ILogger<NativeTenantContainerAdaptor> logger,
            IServiceProvider innerServiceProvider,
            IServiceCollection serviceCollection,
            ContainerRole role = ContainerRole.Root,
            string name = ""
        )
        {
            _logger = logger;
            _innerServiceProvider = innerServiceProvider;
            _serviceCollection = serviceCollection;
            // _scope = scope;
            _id = Guid.NewGuid();
            Role = role;
            if (name == null)
            {
                ContainerName = "NULL";
            }
            else
            {
                ContainerName = name;
            }

            _logger.LogDebug("Container Created: {id}, {containerNAme}, {role}", _id, ContainerName, Role);
        }

        public NativeTenantContainerAdaptor(
            ILogger<NativeTenantContainerAdaptor> logger,
            IServiceScope scope,
            ContainerRole role = ContainerRole.Scoped,
            string name = ""
        ) : this(logger, scope.ServiceProvider, null, role, name)
        {
            _scope = scope;
        }

        public ContainerRole Role { get; set; }


        public string ContainerName { get; set; }
        public Guid ContainerId => _id;

        public ITenantContainerAdaptor CreateScope(string name)
        {
            _logger.LogDebug("Creating scope from container: {id}, {containerName}, {role}", _id, ContainerName, Role);
            var scope = _innerServiceProvider.CreateScope();
            return new NativeTenantContainerAdaptor(_logger, scope, ContainerRole.Scoped, name);
        }

        public ITenantContainerAdaptor CreateChild(string name, Action<IServiceCollection> configureChild)
        {
            if (Role == ContainerRole.Scoped)
            {
                throw new InvalidOperationException("Creating a child container from a scoped container is not allowed.");
            }

            _logger.LogDebug("Creating child container from container: {id}, {containerName}, {role}", _id, ContainerName, Role);
           // IServiceProvider childSp = null;
            var childServiceProvider = this.CreateChildServiceProvider(_serviceCollection, (childServices) =>
            {
                configureChild?.Invoke(childServices);
                
                // support resolving the container adaptor from inside the child container.
                // This lets consumers use this adapted interface to create child containers etc.
                childServices.AddSingleton<ITenantContainerAdaptor>(sp=>
                {
                    //sp.GetRequiredService<ILogger<NativeTenantContainerAdaptor>>();
                    IServiceCollection allServices = new ServiceCollection();
                    var allChildServices = childServices.ToArray();
                    foreach (var cs in allChildServices)
                    {
                        allServices.Add(cs);
                    }
                 
                    var adaptor = new NativeTenantContainerAdaptor(_logger, sp, allServices, ContainerRole.Child, name);
                    return adaptor;
                });
              
            }, s => s.BuildServiceProvider(), ParentSingletonOpenGenericRegistrationsBehaviour.Delegate);
          

            return childServiceProvider.GetRequiredService<ITenantContainerAdaptor>();


            //  return new NativeTenantContainerAdaptor(_logger, childServiceProvider, finalChildServices, ContainerRole.Child, name);
        }

        public async Task<ITenantContainerAdaptor> CreateChildAsync(string name, Func<IServiceCollection, Task> configureChild)
        {
            if (Role == ContainerRole.Scoped)
            {
                throw new InvalidOperationException("Creating a child container from a scoped container is not allowed.");
            }

            _logger.LogDebug("Creating child container from container: {id}, {containerName}, {role}", _id, ContainerName, Role);
            IServiceCollection finalChildServices = null;
            var childServiceProvider = await _serviceCollection.CreateChildServiceProviderAsync(_innerServiceProvider, async (childServices) =>
            {
                // support resolving the container adaptor from inside the child container.
                // This lets consumers use this adapted interface to create child containers etc.
                childServices.AddSingleton<ITenantContainerAdaptor>(sp =>
                {
                    var logger = _logger;
                   // var logger = sp.GetRequiredService<ILogger<NativeTenantContainerAdaptor>>(); // we can't resolve this because "sp" here is a native di container scope.. is this a problem?
                    var adaptor = new NativeTenantContainerAdaptor(logger, sp, childServices, ContainerRole.Child, name);
                    return adaptor;
                });
                if (configureChild != null)
                {
                    await configureChild(childServices);
                }
            }, sp => sp.BuildServiceProvider(), ParentSingletonOpenGenericRegistrationsBehaviour.Delegate);

            return childServiceProvider.GetRequiredService<ITenantContainerAdaptor>();
        }

        public void Dispose()
        {
            _logger.LogDebug("Disposing of container: {id}, {containerNAme}, {role}", _id, ContainerName, Role);

            _scope?.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _innerServiceProvider.GetService(serviceType);
        }

        public IServiceScope CreateScope()
        {
            return CreateScope(string.Empty);
        }

        public IServiceProvider ServiceProvider
        {
            get { return this; }
        }
    }
}