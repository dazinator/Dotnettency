namespace IdentifyRequest
{

    public static class PatternOptionsExtensions
    {
        public static CreatePatternMatcher Literal(this MatcherStrategies options)
        {
            return new CreatePatternMatcher(GetLiteralMatcher);
        }

        private static IPatternMatcher GetLiteralMatcher(string pattern)
        {
            // authorityUriBuilder.Host           
            return new LiteralPatternMatcher(pattern);
        }     

    }
}
