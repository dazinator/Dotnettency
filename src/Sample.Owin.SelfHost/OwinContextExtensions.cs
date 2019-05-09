using DavidLievrouw.OwinRequestScopeContext;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Sample.Owin.SelfHost
{
    // ...

    //public class DefaultDependencyResolver : IDependencyResolver
    //{
    //    private readonly IServiceProvider provider;

    //    public DefaultDependencyResolver(IServiceProvider provider)
    //    {
    //        this.provider = provider;
    //    }

    //    public object GetService(Type serviceType)
    //    {
    //        return provider.GetService(serviceType);
    //    }

    //    public IEnumerable<object> GetServices(Type serviceType)
    //    {
    //        return provider.GetServices(serviceType);
    //    }

    //    public IDependencyScope BeginScope()
    //    {
    //        return this;
    //    }

    //    public void Dispose()
    //    {
    //    }
    //}

    public static class OwinContextExtensions
    {
        public static IServiceScope GetRequestServiceScope(this Microsoft.Owin.IOwinContext owinContext)
        {
            var current = OwinRequestScopeContext.Current;
            current.Items.TryGetValue(typeof(IServiceScope).Name, out object scope);
            return scope as IServiceScope;
        }

        public static IServiceProvider GetRequestServices(this Microsoft.Owin.IOwinContext owinContext)
        {
            var scope = GetRequestServiceScope(owinContext);
            return scope.ServiceProvider;

        }

        public static Task<TTenant> GetTenantAysnc<TTenant>(this Microsoft.Owin.IOwinContext owinContext)
        {
            var scope = GetRequestServices(owinContext);
            return scope.GetRequiredService<Task<TTenant>>();
        }


       
    }
}
