using DotNet.Globbing;

namespace IdentifyRequest.DotNetGlob
{
    public class GlobPatternMatcher : IPatternMatcher
    {
        private Glob _glob;

        public GlobPatternMatcher(string globPattern, GlobOptions options)
        {
            _glob = Glob.Parse(globPattern, options);
        }
        public bool IsMatch(string testValue)
        {
            return _glob.IsMatch(testValue);
        }
    }

}
