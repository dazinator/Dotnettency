using DotNet.Globbing;

namespace Dotnettency
{
    public class GlobPattern : IPatternMatcher
    {

        private Glob _glob;

        public GlobPattern(string globPattern, GlobOptions options)
        {
            _glob = Glob.Parse(globPattern, options);
        }
        public bool IsMatch(string testValue)
        {
            return _glob.IsMatch(testValue);
        }
    }
}
