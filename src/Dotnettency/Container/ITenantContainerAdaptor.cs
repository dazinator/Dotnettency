using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency.Container
{
    public interface ITenantContainerAdaptor : IServiceProvider, IDisposable
    {
        ITenantContainerAdaptor CreateNestedContainer(string Name);
        ITenantContainerAdaptor CreateChildContainer(string Name);

        ITenantContainerAdaptor CreateChildContainerAndConfigure(string Name, IServiceCollection parentServices, Action<IServiceCollection> configure);
        Task<ITenantContainerAdaptor> CreateChildContainerAndConfigureAsync(string Name, IServiceCollection parentServices, Func<IServiceCollection, Task> configure);

        ITenantContainerAdaptor CreateNestedContainerAndConfigure(string Name);

        /// <summary>
        /// Adding services to an already running / configured container is bad. Safer to treat a created container as immutable.
        /// Need to re-design modules system to solve this.
        /// </summary>
        /// <param name="configure"></param>
        void AddServices(Action<IServiceCollection> configure);

        /// <summary>
        /// Used to add services to a container AFTER its initialised.
        /// </summary>
        /// <param name="configure"></param>
     //   void Configure(Action<IServiceCollection> configure);

        string ContainerName { get; }
        Guid ContainerId { get; }
        ContainerRole Role { get; }
    }
}
