using System.Collections.Generic;

namespace IdentifyRequest
{
    public interface IMappingMatcherProvider<TKey>
    {
        IEnumerable<MappingMatcher<TKey>> GetMatchers(MappingOptions<TKey> options);
    }
}
