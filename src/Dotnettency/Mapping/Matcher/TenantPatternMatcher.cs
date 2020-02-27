using System;
using System.Collections.Generic;

namespace Dotnettency
{
    public class TenantPatternMatcher<TKey> : IPatternMatcher
    {
        //  private readonly Func<IServiceProvider, bool> _isEnabled;
        private readonly Func<bool> _checkIsEnabled;
        private readonly IEnumerable<IPatternMatcher> _patterns;

        public TenantPatternMatcher(TKey key, Func<bool> checkIsEnabled, IEnumerable<IPatternMatcher> patterns)
        {
            Key = key;
            _checkIsEnabled = checkIsEnabled;
            _patterns = patterns;
        }

        public TKey Key { get; }

        public bool IsEnabled()
        {
            return _checkIsEnabled?.Invoke() ?? true;            
        }        

        public bool IsMatch(string testValue)
        {
            if(!IsEnabled())
            {
                return false;
            }

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