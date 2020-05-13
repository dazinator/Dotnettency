using DotNet.Globbing;

namespace IdentifyRequest.DotNetGlob
{
    public static class PatternOptionsExtensions
    {
        private static readonly GlobOptions _options = new GlobOptions() { Evaluation = new EvaluationOptions() { CaseInsensitive = true } };

        public static CreatePatternMatcher Glob(this MatcherStrategies matchingOptions)
        {           
            return new CreatePatternMatcher(GetGlobPatternMatcher);
        }      

        private static IPatternMatcher GetGlobPatternMatcher(string pattern)
        {
            // authorityUriBuilder.Host           
            return new GlobPatternMatcher(pattern, _options);
        }
    }
}
