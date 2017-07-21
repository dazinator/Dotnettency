using System;

namespace Dotnettency.Container
{
    public interface ITenantContainerAdaptor : IDisposable
    {
        Lazy<IServiceProvider> ServiceProvider { get; }
        ITenantContainerAdaptor CreateNestedContainer();
    }


}
