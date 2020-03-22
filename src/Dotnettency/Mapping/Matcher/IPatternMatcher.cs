namespace Dotnettency
{
    public interface IPatternMatcher
    {
        bool IsMatch(string testValue);     
    }

    public interface ITenantPatternMatcher : IPatternMatcher
    {
        string FactoryName { get; }
    }
}