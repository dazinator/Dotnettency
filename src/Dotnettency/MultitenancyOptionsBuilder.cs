using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Dotnettency
{

    public class MultitenancyOptionsBuilder<TTenant>
        where TTenant : class
    {

        public MultitenancyOptionsBuilder(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;

            // add default services
            Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Services.AddScoped<ITenantAccessor<TTenant>, TenantAccessor<TTenant>>();
            Services.AddSingleton<ITenantShellCache<TTenant>, ConcurrentDictionaryTenantShellCache<TTenant>>();
            Services.AddSingleton<ITenantShellResolver<TTenant>, TenantShellResolver<TTenant>>();
            Services.AddScoped<TenantDistinguisherAccessor<TTenant>>();
            Services.AddScoped<ITenantShellAccessor<TTenant>, TenantShellAccessor<TTenant>>();

        }

        public IServiceProvider ServiceProvider { get; set; }

        public MultitenancyOptionsBuilder<TTenant> DistinguishTenantsWith<T>()
            where T : class, ITenantDistinguisherFactory<TTenant>
        {
            Services.AddSingleton<ITenantDistinguisherFactory<TTenant>, T>();
            return this;

        }

        public MultitenancyOptionsBuilder<TTenant> DistinguishTenantsWithHostname()
        {
            Services.AddSingleton<ITenantDistinguisherFactory<TTenant>, HostnameTenantDistinguisherFactory<TTenant>>();
            return this;

        }

        public IServiceCollection Services { get; set; }

        public MultitenancyOptionsBuilder<TTenant> GetTenantWith<T>()
       where T : class, ITenantShellFactory<TTenant>
        {
            Services.AddSingleton<ITenantShellFactory<TTenant>, T>();
            return this;

        }

        public MultitenancyOptionsBuilder<TTenant> OnResolveTenant(Func<TenantDistinguisher, TenantShell<TTenant>> factoryMethod)
        {
            var factory = new DelegateTenantShellFactory<TTenant>(factoryMethod);
            Services.AddSingleton<ITenantShellFactory<TTenant>>(factory);
            return this;
        }
    }


}