using System.Collections.Generic;

namespace Dotnettency
{
    public interface ITenantMatcherFactory<TKey>
    {
        IEnumerable<TenantPatternMatcher<TKey>> LoadPaternMatchers(TenantMappingOptions<TKey> options);
    }

}