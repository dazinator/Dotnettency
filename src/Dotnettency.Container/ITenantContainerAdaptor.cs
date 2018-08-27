using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Container
{
    public interface ITenantContainerAdaptor : IServiceProvider, IDisposable
    {
        ITenantContainerAdaptor CreateNestedContainer(string Name);
        ITenantContainerAdaptor CreateChildContainer(string Name);

        ITenantContainerAdaptor CreateChildContainerAndConfigure(string Name, Action<IServiceCollection> configure);
        ITenantContainerAdaptor CreateNestedContainerAndConfigure(string Name, Action<IServiceCollection> configure);


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
