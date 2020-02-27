using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Dotnettency
{
    public class MappedHttpContextTenantIdentifierFactory<TTenant, TKey> : HttpContextTenantIdentifierFactory<TTenant>, IDisposable
      where TTenant : class
    {
        private readonly IHttpContextValueSelector _valueSelector;
        private readonly IOptionsProvider<TenantMappingOptions<TKey>> _optionsMonitor;
        private readonly ITenantMatcherFactory<TKey> _matcherFactory;
        private static readonly string keyTypeName = typeof(TKey).Name;
        private static readonly string identifierPrefix = $"key://{keyTypeName}/";
        private static readonly string identifierFormatString = $"{identifierPrefix}{{0}}";
        private static readonly Uri noTenantUri = new Uri(identifierPrefix);

        private IDisposable _optionsOnChangeToken = null;
        private Lazy<IEnumerable<TenantPatternMatcher<TKey>>> _lazyTenantMatchers;
        private readonly ILogger<MappedHttpContextTenantIdentifierFactory<TTenant, TKey>> _logger;

        public MappedHttpContextTenantIdentifierFactory(
            ILogger<MappedHttpContextTenantIdentifierFactory<TTenant, TKey>> logger,
            IHttpContextProvider httpContextAccessor,
            IHttpContextValueSelector valueSelector,
            IOptionsProvider<TenantMappingOptions<TKey>> optionsMonitor,           
            ITenantMatcherFactory<TKey> matcherFactory) : base(httpContextAccessor)
        {
            _logger = logger;
            _valueSelector = valueSelector;
            _optionsMonitor = optionsMonitor;
            _matcherFactory = matcherFactory;
            SetLazy();
            _optionsOnChangeToken = _optionsMonitor.OnChange((a) =>
            {
                _logger.LogInformation("Change detected for mapping options, reloading.");
                SetLazy(a);
            });
        }

        private void SetLazy(TenantMappingOptions<TKey> options = null)
        {
            var opts = options ?? _optionsMonitor.CurrentValue;
            _lazyTenantMatchers = new Lazy<IEnumerable<TenantPatternMatcher<TKey>>>(() => _matcherFactory.LoadPaternMatchers(opts));
        }

        protected static TenantIdentifier CreateIdentifier(TKey value)
        {
            var ident = new Uri(string.Format(identifierFormatString, value.ToString()));
            return new TenantIdentifier(ident);
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
                    //todo: check condition

                    // we've mapped this url to a particular tenant's key.
                    // store the key in the identifiers URI Path.
                    return CreateIdentifier(tenantMatcher.Key);
                }
            }

            // no match
            return new TenantIdentifier(noTenantUri);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_optionsOnChangeToken != null)
                    {
                        _optionsOnChangeToken.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MappedHttpContextTenantIdentifierFactory()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}