using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dotnettency
{
    public class MapRequestOptionsBuilder<TTenant, TKey>
        where TTenant : class
    {
        private readonly MultitenancyOptionsBuilder<TTenant> _builder;

        public MapRequestOptionsBuilder(MultitenancyOptionsBuilder<TTenant> builder)
        {
            _builder = builder;
            IdentifyWith<MappedHttpContextTenantIdentifierFactory<TTenant, TKey>>();
        }
               

        public MapRequestOptionsBuilder<TTenant, TKey> IdentifyWith<TIdentifierFactory>()
            where TIdentifierFactory: MappedHttpContextTenantIdentifierFactory<TTenant, TKey>
        {
            _builder.IdentifyTenantsWith<TIdentifierFactory>();
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

        public MapRequestOptionsBuilder<TTenant, TKey> InitialiseWith<TTenantShellFactory>()
            where TTenantShellFactory : MappedTenantShellFactory<TTenant, TKey>
        {
            _builder.InitialiseTenant<TTenantShellFactory>();
            return this;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> Initialise(Func<TKey, Task<TTenant>> getTenant)
        {
            var delegateFactory = new DelegateMappedTenantShellFactory<TTenant, TKey>(getTenant);
            _builder.InitialiseTenant(delegateFactory);
            return this;
        }

        /// <summary>
        /// Select a tenant shell factory to use to initialise the tenant's shell, based on the key value, of the key mapped to this request.
        /// </summary>
        /// <param name="factory"></param>
        /// <returns>return null will result in the default <see cref="ITenantShellFactory{TTenant}"/> being used. Return the type of an alternative one to use to override it.</returns>
        public MapRequestOptionsBuilder<TTenant, TKey> OverrideInitialise(Func<TKey, Type> factory)
        {
            Services.AddScoped<ITenantShellFactoryStrategy<TTenant>>(sp =>
            {
                var defaultFactory = sp.GetRequiredService<ITenantShellFactory<TTenant>>();
                return new SelectTenantShellTypeFromKeyFactoryStrategy<TKey, TTenant>(sp, defaultFactory, factory);
            });           
            return this;
        }
      

    }
}