using DotNet.Globbing;
using Dotnettency.Extensions.MappedTenants;
using System.Collections.Generic;
using System.Linq;

namespace Dotnettency
{
    public class DotNetGlobTenantMatcherFactory<TKey> : ITenantMatcherFactory<TKey>
    {
        public virtual IEnumerable<TenantPatternMatcher<TKey>> LoadPaternMatchers(TenantMappingOptions<TKey> options)
        {
            var caseInsensitiveGlobOptions = new GlobOptions();
            caseInsensitiveGlobOptions.Evaluation.CaseInsensitive = true;

            var matchers = new List<TenantPatternMatcher<TKey>>();
            foreach (var item in options?.TenantMappings)
            {
                var key = item.Key;
                //if(key==null)
                //{
                //    throw new NotImplementedException();
                //}
                var patterns = item.Patterns.Select(a => (IPatternMatcher)new GlobPattern(a, caseInsensitiveGlobOptions));
                var tenantPatterMatcher = new TenantPatternMatcher<TKey>(key, patterns);
                matchers.Add(tenantPatterMatcher);
            }

            return matchers.AsEnumerable();
        }
    }
}