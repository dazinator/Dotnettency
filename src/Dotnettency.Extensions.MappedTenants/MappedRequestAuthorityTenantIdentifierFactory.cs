using Dotnettency.Extensions.MappedTenants;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Dotnettency
{
    public class MappedRequestAuthorityTenantIdentifierFactory<TTenant, TKey> : HttpContextTenantIdentifierFactory<TTenant>
      where TTenant : class
    {
        private readonly IOptionsMonitor<TenantMappingOptions<TKey>> _optionsMonitor;
        private readonly TenantMatcherFactory<TKey> _matcherFactory;

        private Lazy<IEnumerable<TenantPatternMatcher<TKey>>> _lazyTenantMatchers;

        public MappedRequestAuthorityTenantIdentifierFactory(
            IHttpContextProvider httpContextAccessor,
            IOptionsMonitor<TenantMappingOptions<TKey>> optionsMonitor,
            TenantMatcherFactory<TKey> matcherFactory) : base(httpContextAccessor)
        {
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
            _lazyTenantMatchers = new Lazy<IEnumerable<TenantPatternMatcher<TKey>>>(() => _matcherFactory.Load(opts));
        }     

        protected override TenantIdentifier GetTenantDistinguisher(HttpContextBase context)
        {
            var uri = context.Request.GetUri();
            var matchers = _lazyTenantMatchers.Value;

            var authorityUriBuilder = new System.UriBuilder(uri);
            authorityUriBuilder.Path = null;
            authorityUriBuilder.Query = null;
            //var uriString = authorityUriBuilder.Uri;

            foreach (var tenantMatcher in matchers)
            {
                if (tenantMatcher.IsMatch(authorityUriBuilder.Host))
                {
                    // set the Path to the mapped tenant key, this additional tidbit of information in the identifier can be used by a custom tenant shell resolver
                    // implementation to more easily return the appropriate tenant shell (based on a lookup of the key)
                    authorityUriBuilder.Path = tenantMatcher.Key.ToString();
                    return new TenantIdentifier(authorityUriBuilder.Uri);
                }
            }

            // no match
            authorityUriBuilder.Host = null;
            return new TenantIdentifier(authorityUriBuilder.Uri);
        }
    }
}