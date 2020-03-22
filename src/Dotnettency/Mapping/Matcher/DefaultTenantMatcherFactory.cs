using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnettency
{
    /// <summary>
    /// The default pattern matcher does not cater for patterns at all! Strings must match based on an ordinal match ignoring casing.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DefaultTenantMatcherFactory<TKey> : ITenantMatcherFactory<TKey>
    {
        private readonly ConditionRegistry _conditionRegistry;

        public DefaultTenantMatcherFactory(ConditionRegistry conditionRegistry)
        {
            _conditionRegistry = conditionRegistry;
        }
        public virtual IEnumerable<TenantPatternMatcher<TKey>> LoadPaternMatchers(TenantMappingOptions<TKey> options)
        {
            var matchers = new List<TenantPatternMatcher<TKey>>();
            foreach (var item in options?.Mappings)
            {
                var key = item.Key;
                var patterns = item.Patterns.Select(a => (IPatternMatcher)new DelegatePatternMatcher((b)=> a.Equals(b, StringComparison.OrdinalIgnoreCase)));
                Func<bool> checkIsEnabled = _conditionRegistry.GetEvaluateCondition(item.Condition?.Name, item.Condition?.RequiredValue ?? false);
                var tenantPatterMatcher = new TenantPatternMatcher<TKey>(key, checkIsEnabled, patterns, item.FactoryName);
                matchers.Add(tenantPatterMatcher);
            }

            return matchers.AsEnumerable();
        }
    }

}