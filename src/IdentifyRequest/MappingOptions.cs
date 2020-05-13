using System.Collections.Generic;

namespace IdentifyRequest
{
    public class MappingOptions<TKey>
    {
        public MappingOptions()
        {
            Mappings = new List<Mapping<TKey>>();
        }
        public List<Mapping<TKey>> Mappings { get; set; }
    }

    public delegate IPatternMatcher CreatePatternMatcher(string pattern);
}
