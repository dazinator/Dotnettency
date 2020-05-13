using System;

namespace IdentifyRequest
{
    public class DelegatePatternMatcherFactory<TKey> : IPatternMatcherFactory<TKey>
    {
        private readonly CreatePatternMatcher _factoryFunc;

        public DelegatePatternMatcherFactory(CreatePatternMatcher factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }
        IPatternMatcher IPatternMatcherFactory<TKey>.Create(string pattern)
        {
            return _factoryFunc.Invoke(pattern) ?? new LiteralPatternMatcher(pattern);
        }
    }
}
