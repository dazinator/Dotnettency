using StructureMap;
using System;

namespace WebExperiment
{

    public class StructureMapTenantContainerAdaptor : ITenantContainerAdaptor
    {
        private readonly IContainer _container;

        public StructureMapTenantContainerAdaptor(IContainer container)
        {
            _container = container;
            ServiceProvider = new Lazy<IServiceProvider>(() =>
            {
                return _container.GetInstance<IServiceProvider>();
            });
        }
        public Lazy<IServiceProvider> ServiceProvider { get; }

        public ITenantContainerAdaptor CreateNestedContainer()
        {
            return new StructureMapTenantContainerAdaptor(_container.GetNestedContainer());
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
    //public class TenantContainerAdaptor : ITenantContainerAdaptor
    //{

    //    //public Lazy<IServiceProvider> ServiceProvider { get; set; }

    //    //private Container _container { get; set; }

    //    private readonly Func<ITenantContainerAdaptor> _createNestedContainer;
    //    private readonly Func<IServiceProvider> _serviceProviderFactory;
    //    private readonly Action _onDispose;

    //    public TenantContainerAdaptor(Func<ITenantContainerAdaptor> createNestedContainer, Func<IServiceProvider> serviceProviderFactory, Action OnDispose)
    //    {
    //        _createNestedContainer = createNestedContainer;
    //        _serviceProviderFactory = serviceProviderFactory;
    //        _onDispose = OnDispose;
    //        ServiceProvider = new Lazy<IServiceProvider>(() =>
    //        {
    //            return _serviceProviderFactory();
    //        });
    //    }

    //    public Lazy<IServiceProvider> ServiceProvider { get; }

    //    public ITenantContainerAdaptor CreateNestedContainer()
    //    {
    //        return _createNestedContainer();
    //    }

    //    public void Dispose()
    //    {
    //        _onDispose();
    //    }
    //}


}
