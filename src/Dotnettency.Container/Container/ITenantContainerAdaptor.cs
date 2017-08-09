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

        ContainerRole Role { get; }

        /// <summary>
        /// Used to add services to a container AFTER its initialised.
        /// </summary>
        /// <param name="configure"></param>
        void Configure(Action<IServiceCollection> configure);

    }

    public enum ContainerRole
    {
        /// <summary>
        /// The root application level container.
        /// </summary>
        Root = 0,
        /// <summary>
        /// A child container, derived from a parent.
        /// </summary>
        Child = 1,
        /// <summary>
        /// A child container that is scoped for some lifetime such as a request.
        /// </summary>
        Scoped = 2
    }


}
