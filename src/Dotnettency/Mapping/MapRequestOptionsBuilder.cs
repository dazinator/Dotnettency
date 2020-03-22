using Dazinator.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class MapRequestOptionsBuilder<TTenant, TKey>
        where TTenant : class
    {
        private readonly MultitenancyOptionsBuilder<TTenant> _builder;
        private readonly ConditionRegistry _conditions = new ConditionRegistry();

        public MapRequestOptionsBuilder(MultitenancyOptionsBuilder<TTenant> builder)
        {
            _builder = builder;
            Identify<MappedHttpContextTenantIdentifierFactory<TTenant, TKey>>();
            SetPatternMatcherFactory<DefaultTenantMatcherFactory<TKey>>();
        }

        private MapRequestOptionsBuilder<TTenant, TKey> Identify<TIdentifierFactory>()
            where TIdentifierFactory : MappedHttpContextTenantIdentifierFactory<TTenant, TKey>
        {
            _builder.Identify<TIdentifierFactory>();
            return this;
        }

        public IServiceCollection Services { get { return _builder.Services; } }

        public MapRequestOptionsBuilder<TTenant, TKey> SelectValue(Func<HttpContextBase, string> selectValue)
        {
            var instance = new HttpContextValueSelectorFunc(selectValue);
            _builder.Services.AddSingleton<IHttpContextValueSelector>(instance);
            return this;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> SelectRequestHost()
        {
            return SelectValue<HostValueSelector>();
        }

        public MapRequestOptionsBuilder<TTenant, TKey> SelectValue<TValueSelector>()
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

        public MapRequestOptionsBuilder<TTenant, TKey> RegisterConditions(Action<ConditionRegistry> registerConditions)
        {
            registerConditions(_conditions);
            return this;
        }
              
        ///// <summary>
        ///// Register a default factory Func that will be used to create TTenant instance.
        ///// </summary>
        ///// <param name="getTenant"></param>
        ///// <returns></returns>
        //public MapRequestOptionsBuilder<TTenant, TKey> TenantFactory<TTenantFactory>(ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<TKey, Task<TTenant>> getTenant = null)
        //    where TTenantFactory : TenantFactory<TTenant, TKey>
        //{
        //    // default named factory (I.e no name) that NamedFactoryTenantShellFactory uses by default to create TTenant instances.
        //    ServiceDescriptor descriptor = null;
        //    if (getTenant == null)
        //    {
        //        descriptor = new ServiceDescriptor(typeof(TenantFactory<TTenant>), typeof(TTenantFactory), lifetime);
        //    }
        //    else
        //    {
        //        var fact = new Func<IServiceProvider, TenantFactory<TTenant, TKey>>(sp =>
        //        {
        //            var delegateFactory = new DelegateTenantFactory<TTenant, TKey>(getTenant);
        //            return delegateFactory;
        //        });

        //        descriptor = new ServiceDescriptor(typeof(TenantFactory<TTenant>), fact, lifetime);
        //    }
        //    Services.Add(descriptor);
        //    return this;
        //}

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