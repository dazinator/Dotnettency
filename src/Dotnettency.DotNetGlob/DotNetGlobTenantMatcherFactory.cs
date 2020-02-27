using DotNet.Globbing;
using Dotnettency.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnettency
{
   

    public class DotNetGlobTenantMatcherFactory<TKey> : ITenantMatcherFactory<TKey>
    {
        private readonly ConditionRegistry _conditionRegistry;

        public DotNetGlobTenantMatcherFactory(ConditionRegistry conditionRegistry)
        {
            _conditionRegistry = conditionRegistry;
        }
        public virtual IEnumerable<TenantPatternMatcher<TKey>> LoadPaternMatchers(TenantMappingOptions<TKey> options)
        {
            var caseInsensitiveGlobOptions = new GlobOptions();
            caseInsensitiveGlobOptions.Evaluation.CaseInsensitive = true;

            var matchers = new List<TenantPatternMatcher<TKey>>();
            foreach (var item in options?.Mappings)
            {
                var key = item.Key;             
                var patterns = item.Patterns.Select(a => (IPatternMatcher)new GlobPattern(a, caseInsensitiveGlobOptions));
                Func<bool> checkIsEnabled = _conditionRegistry.GetEvaluateCondition(item.Condition?.Name, item.Condition?.RequiredValue ?? false);               
                var tenantPatterMatcher = new TenantPatternMatcher<TKey>(key, checkIsEnabled, patterns);
                matchers.Add(tenantPatterMatcher);
            }

            return matchers.AsEnumerable();
        }       
    }

   
}