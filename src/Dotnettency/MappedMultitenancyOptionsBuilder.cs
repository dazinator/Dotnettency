using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class MappedMultitenancyOptionsBuilder<TTenant, TKey> : MultitenancyOptionsBuilder<TTenant>
     where TTenant : class
    {
        public MappedMultitenancyOptionsBuilder(IServiceCollection serviceCollection, Func<IServiceProvider> serviceProviderFactory, IHttpContextProvider httpContextProvider) : base(serviceCollection, serviceProviderFactory, httpContextProvider)
        {
        }

        /// <summary>
        /// Register a default factory Func that will be used to create TTenant instance from the given mapped key.
        /// </summary>
        /// <param name="getTenant"></param>
        /// <returns></returns>
        public MappedMultitenancyOptionsBuilder<TTenant, TKey> Get(Func<TKey, Task<TTenant>> getTenant)
        {
            // default named factory (I.e no name) that NamedFactoryTenantShellFactory uses by default to create TTenant instances.
            Services.AddScoped<TenantFactory<TTenant>>(sp =>
            {
                var delegateFactory = new DelegateTenantFactory<TTenant, TKey>(getTenant);
                return delegateFactory;
            });
            return this;
        }

        /// <summary>
        /// Register a factory method to return a tenant factory, scoped with specified lifetime.
        /// </summary>
        /// <param name="getTenant"></param>
        /// <returns></returns>
        public new MultitenancyOptionsBuilder<TTenant> Get<TTenantFactory>(ServiceLifetime lifetime, Func<IServiceProvider, TTenantFactory> factoryFunc = null)
            where TTenantFactory : TenantFactory<TTenant, TKey>
        {
            // default named factory (I.e no name) that NamedFactoryTenantShellFactory uses by default to create TTenant instances.
            ServiceDescriptor descriptor = null;
            if (factoryFunc == null)
            {
                descriptor = new ServiceDescriptor(typeof(TenantFactory<TTenant>), typeof(TTenantFactory), lifetime);
            }
            else
            {
                descriptor = new ServiceDescriptor(typeof(TenantFactory<TTenant>), factoryFunc, lifetime);
            }
            Services.Add(descriptor);
            return this;
        }
               

    }    
}
