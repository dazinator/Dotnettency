using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Dotnettency.WebApi
{

    public class DefaultDependencyResolver : IDependencyResolver
    {
        protected ITenantContainerAdaptor ServiceProvider;


        public DefaultDependencyResolver(ITenantContainerAdaptor serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IDependencyScope BeginScope()
        {
            return new DefaultIDependencyScope(ServiceProvider.CreateNestedContainer("Web API Scope"));
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return ServiceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ServiceProvider.GetServices(serviceType);
        }
    }
}
