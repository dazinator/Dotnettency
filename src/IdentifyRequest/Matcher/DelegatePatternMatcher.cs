using System;

namespace IdentifyRequest
{
    public class DelegatePatternMatcher : IPatternMatcher
    {
        private readonly string _pattern;
        private readonly Func<string, string, bool> _evaluateIsMatch;

        public DelegatePatternMatcher(string pattern, Func<string, string, bool> evaluateIsMatch)
        {
            _pattern = pattern;
            //  FactoryName = factoryName;
            _evaluateIsMatch = evaluateIsMatch;
        }

        public bool IsMatch(string testValue)
        {
            return _evaluateIsMatch.Invoke(_pattern, testValue);
        }
    }
}
