using System.Collections.Generic;

namespace Dotnettency
{
    public class TenantPatternMatcher<TKey> : IPatternMatcher
    {
        private readonly IEnumerable<IPatternMatcher> _patterns;

        public TenantPatternMatcher(TKey key, IEnumerable<IPatternMatcher> patterns)
        {
            Key = key;
            _patterns = patterns;
        }

        public TKey Key { get; }

        public bool IsMatch(string testValue)
        {
            foreach (var item in _patterns)
            {
                if(item.IsMatch(testValue))
                {
                    return true;
                }
            }

            return false;
        }
    }
}