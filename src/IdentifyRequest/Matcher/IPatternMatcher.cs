namespace IdentifyRequest
{
    public interface IPatternMatcher
    {
        bool IsMatch(string testValue);
    }
}
