using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Dazinator.Extensions.DependencyInjection.ChildContainers;

namespace Dotnettency.Container
{
    public interface ITenantContainerAdaptor : IServiceProvider, IDisposable, IServiceScopeFactory, IServiceScope
    {
        /// <summary>
        /// You can create a new scoped container from an existing container. This is useful for creating a temporary lifetime context to perform some isolated work, where you want everything disposed at the end. Only services registered as "scoped" will have instances
        /// tied to the new container scope when requested in that new scope.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ITenantContainerAdaptor CreateScope(string name);
        /// <summary>
        /// A child container is a new container that is created from an existing container. It will inherit all the services from the parent container, but can override them with its own registrations. This container is not meant to be used
        /// for servicing isolated units of work that repeat over and over (use a scope instead). It is meant to be used for creating a new container that can be customised for some sub-area of the system. It is probably held onto for the lifetime of the application, and scopes can be created from it as needed.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configureChild"></param>
        /// <returns></returns>
        ITenantContainerAdaptor CreateChild(string name, Action<IChildServiceCollection> configureChild);
        /// <summary>
        /// A child container is a new container that is created from an existing container. It will inherit all the services from the parent container, but can override them with its own registrations. This container is not meant to be used
        /// for servicing isolated units of work that repeat over and over (use a scope instead). It is meant to be used for creating a new container that can be customised for some sub-area of the system. It is probably held onto for the lifetime of the application, and scopes can be created from it as needed.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        Task<ITenantContainerAdaptor> CreateChildAsync(string name, Func<IChildServiceCollection, Task> configure);

        string ContainerName { get; }
        Guid ContainerId { get; }
        ContainerRole Role { get; }
    }
}
