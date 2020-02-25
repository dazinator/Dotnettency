using Dotnettency.Extensions.MappedTenants;
using System;
using System.Collections.Generic;

namespace Dotnettency
{

    public class MappedHttpContextTenantIdentifierFactory<TTenant, TKey> : HttpContextTenantIdentifierFactory<TTenant>
      where TTenant : class
    {
        private readonly IHttpContextValueSelector _valueSelector;
        private readonly IOptionsProvider<TenantMappingOptions<TKey>> _optionsMonitor;
        private readonly ITenantMatcherFactory<TKey> _matcherFactory;
        private static readonly string keyTypeName = typeof(TKey).Name;
        private static readonly string identifierPrefix = $"key://{keyTypeName}/";
        private static readonly string identifierFormatString = $"{identifierPrefix}{{0}}";
        private static readonly Uri noTenantUri = new Uri(identifierPrefix);


        private Lazy<IEnumerable<TenantPatternMatcher<TKey>>> _lazyTenantMatchers;

        public MappedHttpContextTenantIdentifierFactory(
            IHttpContextProvider httpContextAccessor,
            IHttpContextValueSelector valueSelector,
            IOptionsProvider<TenantMappingOptions<TKey>> optionsMonitor,
            ITenantMatcherFactory<TKey> matcherFactory) : base(httpContextAccessor)
        {
            _valueSelector = valueSelector;
            _optionsMonitor = optionsMonitor;
            _matcherFactory = matcherFactory;
            SetLazy();
            _optionsMonitor.OnChange((a) =>
            {
                SetLazy(a);
            });
        }

        private void SetLazy(TenantMappingOptions<TKey> options = null)
        {
            var opts = options ?? _optionsMonitor.CurrentValue;
            _lazyTenantMatchers = new Lazy<IEnumerable<TenantPatternMatcher<TKey>>>(() => _matcherFactory.LoadPaternMatchers(opts));
        }

        protected override TenantIdentifier GetTenantIdentifier(HttpContextBase context)
        {
            var matchers = _lazyTenantMatchers.Value;

            // we are to return a URI as an identifier for this tenant, which will get used as a cache key.
            foreach (var tenantMatcher in matchers)
            {
                var valueToMap = _valueSelector.SelectValue(context);
                if (tenantMatcher.IsMatch(valueToMap))
                {
                    // we've mapped this url to a particular tenant's key.
                    // store the key in the identifiers URI Path.
                    var ident = new Uri(string.Format(identifierFormatString, tenantMatcher.Key.ToString()));
                    return new TenantIdentifier(ident);
                }
            }

            // no match
            return new TenantIdentifier(noTenantUri);
        }
    }
}