using Dazinator.Extensions.DependencyInjection;
using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
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
            AddDefaultServices(serviceCollection);

        }

        protected MultitenancyOptionsBuilder(IServiceCollection serviceCollection, Func<IServiceProvider> serviceProviderFactory, IHttpContextProvider httpContextProvider)
        {
            Services = serviceCollection;
            ServiceProviderFactory = serviceProviderFactory;
            HttpContextProvider = HttpContextProvider;
        }

        protected virtual void AddDefaultServices(IServiceCollection services)
        {
            // Add default services
            // Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITenantAccessor<TTenant>, TenantAccessor<TTenant>>();

            // Tenant shell cache is special in that it houses the tenant shell for each tenant, and each 
            // tenant shell has state that needs to be kept local to the application (i.e the tenant's container or middleware pipeline.)
            // Therefore it should always be a local / in-memory based cache as will have fundamentally non-serialisable state.
            services.AddSingleton<ITenantShellCache<TTenant>, ConcurrentDictionaryTenantShellCache<TTenant>>();
            services.AddSingleton<ITenantShellResolver<TTenant>, TenantShellResolver<TTenant>>();
            services.AddScoped<TenantIdentifierAccessor<TTenant>>();
            services.AddScoped<ITenantShellAccessor<TTenant>, TenantShellAccessor<TTenant>>();
            services.AddScoped<ITenantShellRestarter<TTenant>, TenantShellRestarter<TTenant>>();

            // By default, we use a URI from the request to identify tenants.
            // Services.AddSingleton<ITenantDistinguisherFactory<TTenant>, RequestAuthorityTenantDistinguisherFactory<TTenant>>();

            // Support injection of TTenant (has side effect that may block during injection)
            services.AddScoped(sp =>
            {
                var accessor = sp.GetRequiredService<ITenantAccessor<TTenant>>();
                return accessor.CurrentTenant.Value.Result;
            });

            // Support injection of Task<TTenant> - a convenience that allows non blocking access to tenant when required 
            // minor contention on Lazy<>
            services.AddScoped(sp =>
            {
                return sp.GetRequiredService<ITenantAccessor<TTenant>>().CurrentTenant.Value;
            });

            // This is used to swap out RequestServices, and then swap it back once disposed.
            // This was originally only needed for tenant containers, but moved to default services because 
            // OWIN envoronments may want to use this to set / get a request scoped IServiceProvider, even if not using per tenant containers.
            // As OWIN doesn't have a mechanism / concept for RequestServices of its own, unlike ASP.NET Core.
            services.AddScoped<RequestServicesSwapper<TTenant>>();

            // Default options provider primarily used for tenant mappings.
            // When using AddAspNetCore this gets overridden with a superior implemenations based on the 
            // aspnet core options system, that can do things like change notifications etc.
            SetGenericOptionsProvider(typeof(BasicOptionsProvider<>));

            // Default shell factory, gets tenant instances from named factories that you register (or default factory with no name)
            services.AddScoped<ITenantShellFactory<TTenant>, NamedFactoryTenantShellFactory<TTenant>>();
           
            // Todo: In future - get rid of this default empty registration, as when we want to use
            // named factories it ends up being registered again resulting in multiple registrations.
            // We are resorting to this at the moments so things don't fail when no named factories have been explicitly registered.
            services.AddNamed<TenantFactory<TTenant>>(a=> { });
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

        /// <summary>
        /// Identify tenants from HttpContext using a configurable mapping.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public MappedMultitenancyOptionsBuilder<TTenant, TKey> Map<TKey>(Action<MapRequestOptionsBuilder<TTenant, TKey>> configureOptions)
        {
            var optionsBuilder = new MapRequestOptionsBuilder<TTenant, TKey>(this);
            configureOptions?.Invoke(optionsBuilder);
            optionsBuilder.Build();
            return new MappedMultitenancyOptionsBuilder<TTenant, TKey>(this.Services, this.ServiceProviderFactory, this.HttpContextProvider);
        }

        /// <summary>
        /// Identify tenants through your own service. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public MultitenancyOptionsBuilder<TTenant> Identify<T>()
            where T : class, ITenantIdentifierFactory<TTenant>
        {
            Services.AddSingleton<ITenantIdentifierFactory<TTenant>, T>();
            return this;
        }

        /// <summary>
        /// Identify tenants using your own Func.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public MultitenancyOptionsBuilder<TTenant> Identify(Func<Task<TenantIdentifier>> factory)
        {
            var delegateFactory = new DelegateTenantIdentifierFactory<TTenant>(factory);
            Services.AddSingleton<ITenantIdentifierFactory<TTenant>>(delegateFactory);
            return this;
        }

        /// <summary>
        /// Initialse the tenant shell for a tenant identifier using a Func.
        /// </summary>
        /// <param name="factoryFunc"></param>
        /// <returns></returns>
        /// <remarks>This will replace the default ITenantShellFactory that relies on TenantFactory<typeparamref name="TTenant"/> being registered.</remarks>
        public MultitenancyOptionsBuilder<TTenant> InitialiseShell(Func<TenantIdentifier, TenantShell<TTenant>> factoryFunc)
        {
            Services.AddSingleton<ITenantShellFactory<TTenant>>(new DelegateTenantShellFactory<TTenant>(factoryFunc));
            return this;
        }

        /// <summary>
        /// Initialise the tenant shell for a tenant using your own custom implementation.
        /// </summary>
        /// <param name="factoryFunc"></param>
        /// <returns></returns>
        /// <remarks>Overrides the default TenantShellFactory.</remarks>
        private MultitenancyOptionsBuilder<TTenant> InitialiseShell<TTenantShellFactory>()
            where TTenantShellFactory : NamedFactoryTenantShellFactory<TTenant>
        {
            Services.AddScoped<ITenantShellFactory<TTenant>, TTenantShellFactory>();
            return this;
        }

        /// <summary>
        /// Register a default factory Func that will be used to create TTenant instance.
        /// </summary>
        /// <param name="getTenant"></param>
        /// <returns></returns>
        public MultitenancyOptionsBuilder<TTenant> Get(Func<TenantIdentifier, Task<TTenant>> getTenant)
        {
            // default named factory (I.e no name) that NamedFactoryTenantShellFactory uses by default to create TTenant instances.
            Services.AddScoped<TenantFactory<TTenant>>(sp =>
            {
                var delegateFactory = new DelegateTenantFactory<TTenant>(getTenant);
                return delegateFactory;
            });
            return this;
        }

        /// <summary>
        /// Register a factory method to return a Tenant Factory with the specified lifetiem scope.
        /// </summary>
        /// <param name="getTenant"></param>
        /// <returns></returns>
        public MultitenancyOptionsBuilder<TTenant> Get<TTenantFactory>(ServiceLifetime lifetime, Func<IServiceProvider, TTenantFactory> factoryFunc = null)
            where TTenantFactory : TenantFactory<TTenant>
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

        /// <summary>
        /// Register named factories that will be used to create TTenant instances. The one that will be used depends on the FactoryName set on the TenantIdentifier during the Identify() stage.
        /// </summary>
        /// <param name="getTenant"></param>
        /// <returns></returns>
        public MultitenancyOptionsBuilder<TTenant> NamedGet(Action<NamedServiceRegistry<TenantFactory<TTenant>>> configure)
        {
            Services.AddNamed<TenantFactory<TTenant>>(configure);
            return this;
        }

    }
}
