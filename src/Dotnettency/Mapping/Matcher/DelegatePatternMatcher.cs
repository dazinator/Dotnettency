using System;

namespace Dotnettency
{
    public class DelegatePatternMatcher : IPatternMatcher
    {
        private readonly Func<string, bool> _evaluateIsMatch;

        public DelegatePatternMatcher(Func<string, bool> evaluateIsMatch)
        {
          //  FactoryName = factoryName;
            _evaluateIsMatch = evaluateIsMatch;
        }


        public string FactoryName { get; }

        public bool IsMatch(string testValue)
        {
            return _evaluateIsMatch.Invoke(testValue);
        }
    }

}