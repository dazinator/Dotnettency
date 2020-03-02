using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{

    public class MultitenancyOptionsBuilder<TTenant>
        where TTenant : class
    {
        public MultitenancyOptionsBuilder(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;
            AddDefaultServices(Services);
        }

        protected virtual void AddDefaultServices(IServiceCollection serviceCollection)
        {
            // Add default services
            // Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            Services.AddScoped<ITenantAccessor<TTenant>, TenantAccessor<TTenant>>();

            // Tenant shell cache is special in that it houses the tenant shell for each tenant, and each 
            // tenant shell has state that needs to be kept local to the application (i.e the tenant's container or middleware pipeline.)
            // Therefore it should always be a local / in-memory based cache as will have fundamentally non-serialisable state.
            Services.AddSingleton<ITenantShellCache<TTenant>, ConcurrentDictionaryTenantShellCache<TTenant>>();
            Services.AddSingleton<ITenantShellResolver<TTenant>, TenantShellResolver<TTenant>>();
            Services.AddScoped<TenantIdentifierAccessor<TTenant>>();
            Services.AddScoped<ITenantShellAccessor<TTenant>, TenantShellAccessor<TTenant>>();
            Services.AddScoped<ITenantShellRestarter<TTenant>, TenantShellRestarter<TTenant>>();

            // By default, we use a URI from the request to identify tenants.
            // Services.AddSingleton<ITenantDistinguisherFactory<TTenant>, RequestAuthorityTenantDistinguisherFactory<TTenant>>();

            // Support injection of TTenant (has side effect that may block during injection)
            Services.AddScoped(sp =>
            {
                var accessor = sp.GetRequiredService<ITenantAccessor<TTenant>>();
                return accessor.CurrentTenant.Value.Result;
            });

            // Support injection of Task<TTenant> - a convenience that allows non blocking access to tenant when required 
            // minor contention on Lazy<>
            Services.AddScoped(sp =>
            {
                return sp.GetRequiredService<ITenantAccessor<TTenant>>().CurrentTenant.Value;
            });

            // This is used to swap out RequestServices, and then swap it back once disposed.
            // This was originally only needed for tenant containers, but moved to default services because 
            // OWIN envoronments may want to use this to set / get a request scoped IServiceProvider, even if not using per tenant containers.
            // As OWIN doesn't have a mechanism / concept for RequestServices of its own, unlike ASP.NET Core.
            Services.AddScoped<RequestServicesSwapper<TTenant>>();

            // Default options provider primarily used for tenant mappings.
            // When using AddAspNetCore this gets overridden with a superior implemenations based on the 
            // aspnet core options system, that can do things like change notifications etc.
            SetGenericOptionsProvider(typeof(BasicOptionsProvider<>));

            // Default startegy just always uses the tenant shell factory registered with di.
            InitialiseTenantStrategy<DefaultTenantShellFactoryStrategy<TTenant>>();
        }

        public Func<IServiceProvider> ServiceProviderFactory { get; set; }
        public IServiceCollection Services { get; set; }
        public IHttpContextProvider HttpContextProvider { get; set; }

        public MultitenancyOptionsBuilder<TTenant> SetHttpContextProvider(IHttpContextProvider provider)
        {
            HttpContextProvider = provider;
            Services.AddSingleton<IHttpContextProvider>(provider);
            return this;
        }

        /// <summary>
        /// Call this to set an options provider. Typically this is set up per platform.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MultitenancyOptionsBuilder<TTenant> SetGenericOptionsProvider(Type optionsProviderOpenGenericType)
        {
            Services.AddSingleton(typeof(IOptionsProvider<>), optionsProviderOpenGenericType);
            return this;
        }

       
        public MultitenancyOptionsBuilder<TTenant> IdentifyFromHttpContext<TKey>(Action<MapRequestOptionsBuilder<TTenant, TKey>> configureOptions)
        {
            var optionsBuilder = new MapRequestOptionsBuilder<TTenant, TKey>(this);
            configureOptions?.Invoke(optionsBuilder);
            optionsBuilder.Build();
            return this;
        }

        /// <summary>
        /// Call this to override the service used to provide a URI for the current request. The URI is used as an identifier
        /// for a tenant to be loaded.    
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MultitenancyOptionsBuilder<TTenant> IdentifyTenantsWith<T>()
            where T : class, ITenantIdentifierFactory<TTenant>
        {
            Services.AddSingleton<ITenantIdentifierFactory<TTenant>, T>();
            return this;
        }

        /// <summary>
        /// Identify tenants based on the Request Authority Uri.  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MultitenancyOptionsBuilder<TTenant> IdentifyTenantsWithRequestAuthorityUri()
        {
            IdentifyTenantsWith<RequestAuthorityTenantIdentifierFactory<TTenant>>();
            return this;
        }

        
        public MultitenancyOptionsBuilder<TTenant> IdentifyTenantTask(Func<Task<TenantIdentifier>> factory)
        {
            var delegateFactory = new DelegateTenantIdentifierFactory<TTenant>(factory);
            Services.AddSingleton<ITenantIdentifierFactory<TTenant>>(delegateFactory);
            return this;
        }

        public MultitenancyOptionsBuilder<TTenant> InitialiseTenantStrategy<T>()
           where T : class, ITenantShellFactoryStrategy<TTenant>
        {
            Services.AddScoped<ITenantShellFactoryStrategy<TTenant>, T>();
            return this;
        }

        public MultitenancyOptionsBuilder<TTenant> InitialiseTenantStrategy<TDependency>(Func<TDependency, Type> chooseFactoryType)
        {
            Services.AddScoped<ITenantShellFactoryStrategy<TTenant>>(sp =>
            {
                return new SelectTypeTenantShellFactoryStrategy<TDependency, TTenant>(sp, chooseFactoryType);
            });
            return this;
        }


        public MultitenancyOptionsBuilder<TTenant> InitialiseTenant<T>()
            where T : class, ITenantShellFactory<TTenant>
        {
            Services.AddScoped<ITenantShellFactory<TTenant>, T>();
            return this;
        }

        public MultitenancyOptionsBuilder<TTenant> InitialiseTenant<T>(T factory)
           where T : class, ITenantShellFactory<TTenant>
        {
            Services.AddSingleton<ITenantShellFactory<TTenant>>(factory);
            return this;
        }

        public MultitenancyOptionsBuilder<TTenant> InitialiseTenant(Func<TenantIdentifier, TenantShell<TTenant>> factoryMethod)
        {
            var factory = new DelegateTenantShellFactory<TTenant>(factoryMethod);
            return InitialiseTenant(factory);
        }      

    }
}
