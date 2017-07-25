using System;

namespace Dotnettency.Container
{
    public interface ITenantContainerAdaptor : IDisposable
    {
        Lazy<IServiceProvider> ServiceProvider { get; }
        ITenantContainerAdaptor CreateNestedContainer();

        string ContainerName { get; }
        Guid ContainerId { get; }

    }


}
