using Dotnettency.Extensions.MappedTenants;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dotnettency
{
    public class MapRequestOptionsBuilder<TTenant, TKey>
    {
        private readonly IServiceCollection _services;

        public MapRequestOptionsBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> MapValue(Func<HttpContextBase, string> selectValue)
        {
            var instance = new HttpContextValueSelectorFunc(selectValue);
            _services.AddSingleton<IHttpContextValueSelector>(instance);
            return this;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> MapRequestHost()
        {
            return MapValue<HostValueSelector>();
        }

        public MapRequestOptionsBuilder<TTenant, TKey> MapValue<TValueSelector>()
            where TValueSelector : class, IHttpContextValueSelector
        {
            _services.AddSingleton<IHttpContextValueSelector, TValueSelector>();
            return this;
        }

        public MapRequestOptionsBuilder<TTenant, TKey> SetPatternMatcherFactory<TMatcherFactory>()
            where TMatcherFactory : class, ITenantMatcherFactory<TKey>
        {
            _services.AddSingleton<ITenantMatcherFactory<TKey>, TMatcherFactory>();
            return this;
        }

        public IServiceCollection Services { get { return _services; } }
        //public MapRequestOptionsBuilder<TTenant, TKey> ConfigureMap()
        //{

        //    Services
        //}

    }
}