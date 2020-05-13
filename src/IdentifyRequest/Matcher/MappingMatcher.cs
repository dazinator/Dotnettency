using System;
using System.Collections.Generic;

namespace IdentifyRequest
{
    public class MappingMatcher<TKey> 
    {
        //  private readonly Func<IServiceProvider, bool> _isEnabled;
        private readonly Func<bool> _checkIsEnabled;
        private readonly IEnumerable<IPatternMatcher> _patterns;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapping">The mapping to return should any of the patterns in this composite match.</param>
        /// <param name="checkIsEnabled">A func that can be used to disable this matcher. It is evaluated before any match is performed.</param>
        /// <param name="patterns">The patterns that form this composite.</param>
        /// <param name="factoryName"></param>
        public MappingMatcher(Mapping<TKey> mapping, Func<bool> checkIsEnabled, IEnumerable<IPatternMatcher> patterns)
        {
            _checkIsEnabled = checkIsEnabled;
            _patterns = patterns;
            Mapping = mapping;
        }

        public Mapping<TKey> Mapping { get; }

        public bool IsEnabled()
        {
            return _checkIsEnabled?.Invoke() ?? true;
        }

        public bool IsMatch(string testValue)
        {
            if (!IsEnabled())
            {
                return false;
            }

            foreach (var item in _patterns)
            {
                if (item.IsMatch(testValue))
                {
                    return true;
                }
            }

            return false;
        }
    }

}
