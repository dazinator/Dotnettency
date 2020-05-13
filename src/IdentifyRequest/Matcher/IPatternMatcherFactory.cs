namespace IdentifyRequest
{
    public interface IPatternMatcherFactory<TKey>
    {
        IPatternMatcher Create(string pattern);
    }   
}
