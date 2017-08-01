using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency.Container
{
    public interface ITenantContainerAdaptor : IDisposable
    {
        IServiceProvider GetServiceProvider();
        ITenantContainerAdaptor CreateNestedContainer();
        ITenantContainerAdaptor CreateChildContainer();

        string ContainerName { get; }
        Guid ContainerId { get; }

        /// <summary>
        /// Used to add services to a container AFTER its initialised.
        /// </summary>
        /// <param name="configure"></param>
        void Configure(Action<IServiceCollection> configure);

    }


}
