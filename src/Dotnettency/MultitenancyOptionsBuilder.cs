using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public class MultitenancyOptionsBuilder<TTenant>
        where TTenant : class
    {
        public Func<IServiceProvider> ServiceProviderFactory { get; set; }
        public IServiceCollection Services { get; set; }

        public MultitenancyOptionsBuilder(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;

            // Add default services
            Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Services.AddScoped<ITenantAccessor<TTenant>, TenantAccessor<TTenant>>();
            
            // Tenant shell cache is special in that it houses the tenant shell for each tenant, and each 
            // tenant shell has state that needs to be kept local to the application (i.e the tenant's container or middleware pipeline.)
            // Therefore it should always be a local / in-memory based cache as will have will have fundamentally non-serialisable state.
            Services.AddSingleton<ITenantShellCache<TTenant>, ConcurrentDictionaryTenantShellCache<TTenant>>();
            Services.AddSingleton<ITenantShellResolver<TTenant>, TenantShellResolver<TTenant>>();
            Services.AddScoped<TenantDistinguisherAccessor<TTenant>>();
            Services.AddScoped<ITenantShellAccessor<TTenant>, TenantShellAccessor<TTenant>>();

            // By default, we use a URI from the request to identify tenants.
            Services.AddSingleton<ITenantDistinguisherFactory<TTenant>, RequestAuthorityTenantDistinguisherFactory<TTenant>>();

            // Support injection of TTenant (has side effect that may block during injection)
            Services.AddScoped(sp => {
                var accessor = sp.GetRequiredService<ITenantAccessor<TTenant>>();
                return accessor.CurrentTenant.Value.Result;
            });

            // Support injection of Task<TTenant> - a convenience that allows non blocking access to tenant when required 
            // minor contention on Lazy<>
            Services.AddScoped(sp => {
                return sp.GetRequiredService<ITenantAccessor<TTenant>>().CurrentTenant.Value;
            });
        }
        
        /// <summary>
        /// Call this to override the service used to provide a URI for the current request. The URI is used as an identifier
        /// for a tenant to be loaded.    
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MultitenancyOptionsBuilder<TTenant> DistinguishTenantsWith<T>()
            where T : class, ITenantDistinguisherFactory<TTenant>
        {
            Services.AddSingleton<ITenantDistinguisherFactory<TTenant>, T>();
            return this;
        }     

        public MultitenancyOptionsBuilder<TTenant> InitialiseTenant<T>()
            where T : class, ITenantShellFactory<TTenant>
        {
            Services.AddSingleton<ITenantShellFactory<TTenant>, T>();
            return this;
        }

        public MultitenancyOptionsBuilder<TTenant> InitialiseTenant(Func<TenantDistinguisher, TenantShell<TTenant>> factoryMethod)
        {
            var factory = new DelegateTenantShellFactory<TTenant>(factoryMethod);
            Services.AddSingleton<ITenantShellFactory<TTenant>>(factory);
            return this;
        }
    }
}
