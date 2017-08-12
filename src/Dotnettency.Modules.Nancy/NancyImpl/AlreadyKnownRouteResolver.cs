using Nancy;
using Nancy.Configuration;
using System.Linq;
using nancyrouting = global::Nancy.Routing;

namespace Dotnettency.Modules.Nancy
{
    public class AlreadyKnownRouteResolver : nancyrouting.IRouteResolver
    {
        private readonly global::Nancy.Routing.Route _route;
        private readonly INancyModule _module;
        private readonly nancyrouting.Trie.MatchResult _matchResult;
        private readonly GlobalizationConfiguration _globalizationConfiguraton;

        public AlreadyKnownRouteResolver()
        {
        }

        public AlreadyKnownRouteResolver(INancyEnvironment environment, nancyrouting.Route route, INancyModule module, nancyrouting.Trie.MatchResult matchResult)
        {
            _globalizationConfiguraton = environment.GetValue<GlobalizationConfiguration>();
            _route = route;
            _module = module;
            _matchResult = matchResult;
        }

        public nancyrouting.ResolveResult Resolve(NancyContext context)
        {
            return BuildResult(context, _matchResult);
        }

        private nancyrouting.ResolveResult BuildResult(NancyContext context, nancyrouting.Trie.MatchResult result)
        {

            context.NegotiationContext.SetModule(_module);
            var route = _module.Routes.ElementAt(result.RouteIndex);
            var parameters = DynamicDictionary.Create(result.Parameters, _globalizationConfiguraton);

            return new nancyrouting.ResolveResult
            {
                Route = route,
                Parameters = parameters,
                Before = _module.Before,
                After = _module.After,
                OnError = _module.OnError
            };

        }


    }


  
}