using Dotnettency.Container;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Dotnettency.WebApi
{
    public class DefaultIDependencyScope : IDependencyScope
    {
        private readonly ITenantContainerAdaptor _serviceScope;

        public DefaultIDependencyScope(ITenantContainerAdaptor serviceScope)
        {
            _serviceScope = serviceScope;
        }

        public void Dispose()
        {
            _serviceScope.Dispose();
        }

        public object GetService(Type serviceType)
        {
            return _serviceScope.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _serviceScope.GetServices(serviceType);
        }
    }
}
