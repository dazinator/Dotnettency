using System;

namespace WebExperiment
{
    public interface ITenantContainerAdaptor : IDisposable
    {
        Lazy<IServiceProvider> ServiceProvider { get; }
        ITenantContainerAdaptor CreateNestedContainer();
    }


}
