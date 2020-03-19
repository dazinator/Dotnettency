using Dazinator.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    //public static class NamedRegistryExtensions
    //{
    //    public NamedServiceRegistry<TService> AddDefault<TService>(this NamedServiceRegistry<TService> registry)
    //    {
    //        registry.add
    //    }
    //}

    public class MapRequestOptionsBuilder<TTenant, TKey>
        where TTenant : class
    {
        private readonly MultitenancyOptionsBuilder<TTenant> _builder;
        private readonly ConditionRegistry _conditions = new ConditionRegistry();
        // private readonly TenantFactoryRegistry<TTenant, TKey> _tenantFactoryTypes = new TenantFactoryRegistry<TTenant, TKey>();

        public MapRequestOptionsBuilder(MultitenancyOptionsBuilder<TTenant> builder)
        {
            _builder = builder;
            Identify<MappedHttpContextTenantIdentifierFactory<TTenant, TKey>>();
            InitialiseShell<MappedTenantShellFactory<TTenant, TKey>>();
        }

        public MapRequestOptionsBuilder<TTenant, TKey> Identify(Func<Task<TenantIdentifier>> factory)
        {
            _builder.Identify(factory);
            return this;
        }


        public MapRequestOptionsBuilder<TTenant, TKey> Identify<TIdentifierFactory>()
            where TIdentifierFactory : MappedHttpContextTenantIdentifierFactory<TTenant, TKey>
        {
            _builder.Identify<TIdentifierFactory>();
            return this;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> MapValue(Func<HttpContextBase, string> selectValue)
        {
            var instance = new HttpContextValueSelectorFunc(selectValue);
            _builder.Services.AddSingleton<IHttpContextValueSelector>(instance);
            return this;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> MapRequestHost()
        {
            return MapValue<HostValueSelector>();
        }

        public MapRequestOptionsBuilder<TTenant, TKey> MapValue<TValueSelector>()
            where TValueSelector : class, IHttpContextValueSelector
        {
            _builder.Services.AddSingleton<IHttpContextValueSelector, TValueSelector>();
            return this;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> SetPatternMatcherFactory<TMatcherFactory>()
            where TMatcherFactory : class, ITenantMatcherFactory<TKey>
        {
            _builder.Services.AddSingleton<ITenantMatcherFactory<TKey>, TMatcherFactory>();
            return this;
        }

        public IServiceCollection Services { get { return _builder.Services; } }

        public MapRequestOptionsBuilder<TTenant, TKey> InitialiseShell<TTenantShellFactory>()
            where TTenantShellFactory : MappedTenantShellFactory<TTenant, TKey>
        {
            Services.AddScoped<ITenantShellFactory<TTenant>, TTenantShellFactory>();
            return this;
        }      


        ///// <summary>
        ///// Select a tenant shell factory to use to initialise the tenant's shell, based on the key value, of the key mapped to this request.
        ///// </summary>
        ///// <param name="factory"></param>
        ///// <returns>return null will result in the default <see cref="ITenantShellFactory{TTenant}"/> being used. Return the type of an alternative one to use to override it.</returns>
        //public MapRequestOptionsBuilder<TTenant, TKey> OverrideInitialise(Func<TKey, Type> factory)
        //{
        //    Services.AddScoped<ITenantShellFactoryStrategy<TTenant>>(sp =>
        //    {
        //        var defaultFactory = sp.GetRequiredService<ITenantShellFactory<TTenant>>();
        //        return new SelectTenantShellTypeFromKeyFactoryStrategy<TKey, TTenant>(sp, defaultFactory, factory);
        //    });
        //    return this;
        //}

        public MapRequestOptionsBuilder<TTenant, TKey> RegisterConditions(Action<ConditionRegistry> registerConditions)
        {
            registerConditions(_conditions);
            return this;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> NamedFactories(Action<NamedServiceRegistry<TenantFactory<TTenant, TKey>>> registerTenantFactories)
        {
            _builder.Services.AddNamed<TenantFactory<TTenant, TKey>>(registerTenantFactories);
            return this;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> Factory(Func<TKey, Task<TTenant>> getTenant)
        {
            Services.AddScoped<TenantFactory<TTenant, TKey>>(sp =>
            {
                var delegateFactory = new DelegateTenantFactory<TTenant, TKey>(getTenant);
                return delegateFactory;
            });
            return this;
        }

        public void Build()
        {
            _builder.Services.AddSingleton<ConditionRegistry>(sp =>
            {
                _conditions.ServiceProvider = sp;
                return _conditions;
            });
        }


    }
}