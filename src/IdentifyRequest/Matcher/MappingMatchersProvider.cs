using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentifyRequest
{
    /// <summary>
    /// The default pattern matcher does not cater for patterns at all! Strings must match based on an ordinal match ignoring casing.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class MappingMatcherProvider<TKey> : IMappingMatcherProvider<TKey>
    {
        private readonly ConditionRegistry _conditionRegistry;
        private readonly IPatternMatcherFactory<TKey> _patternMatcherFactory;

        public MappingMatcherProvider(ConditionRegistry conditionRegistry, IPatternMatcherFactory<TKey> patternMatcherFactory)
        {
            _conditionRegistry = conditionRegistry;
            _patternMatcherFactory = patternMatcherFactory;
        }
        public virtual IEnumerable<MappingMatcher<TKey>> GetMatchers(MappingOptions<TKey> options)
        {
            var matchers = new List<MappingMatcher<TKey>>();
            foreach (var item in options?.Mappings)
            {
                var key = item.Key;
                var patterns = item.Patterns.Select(a => _patternMatcherFactory.Create(a));
                Func<bool> checkIsEnabled = _conditionRegistry.GetEvaluateCondition(item.Condition?.Name, item.Condition?.RequiredValue ?? false);
                var tenantPatterMatcher = new MappingMatcher<TKey>(item, checkIsEnabled, patterns);
                matchers.Add(tenantPatterMatcher);
            }

            return matchers.AsEnumerable();
        }
    }


}
