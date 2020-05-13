using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text.RegularExpressions;
using System;
using IdentifyRequest.DotNetGlob;

namespace IdentifyRequest.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Can_Match_Literal_HostNoPort()
        {
            var services = new ServiceCollection();
            services.Configure<MappingOptions<int>>((a) => a.Mappings.Add(new Mapping<int>()
            {
                Key = 1,
                Patterns = new string[] { "localhost" }
            }));

            AddIdentifyRequest(services, Strategies.Selector.HostNoPort(), Strategies.Matcher.Literal());

            var sp = services.BuildServiceProvider();

            AssertMatchRequest(sp, "localhost", 5000);
        }

        [Fact]
        public void Can_Match_Glob_HostNoPort()
        {
            var services = new ServiceCollection();
            services.Configure<MappingOptions<int>>((a) => a.Mappings.Add(new Mapping<int>()
            {
                Key = 1,
                Patterns = new string[] { "*.localhost" }
            }));

            AddIdentifyRequest(services, Strategies.Selector.HostNoPort(), Strategies.Matcher.Glob());

            var sp = services.BuildServiceProvider();

            AssertMatchRequest(sp, "foo.localhost", 5000);
        }


        private void AssertMatchRequest(ServiceProvider sp, string hostName, int port, Action<string, MappingMatcher<int>> onMatched = null)
        {
            var mappingProvider = sp.GetRequiredService<IMappingMatcherProvider<int>>();
            var mappingOptions = sp.GetRequiredService<IOptions<MappingOptions<int>>>();
            var matchers = mappingProvider.GetMatchers(mappingOptions.Value);
            var valueSelector = sp.GetRequiredService<SelectValue>();

            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;
            request.Scheme = "http";
            request.Host = new HostString(hostName, port);

            var valueToMap = valueSelector(httpContext);

            foreach (var item in matchers)
            {
                if (item.IsMatch(valueToMap))
                {
                    onMatched?.Invoke(valueToMap, item);
                    return;
                }
            }

            throw new Exception("No match");

        }

        public IServiceCollection AddIdentifyRequest(IServiceCollection services, SelectValue selectValueStrategy, CreatePatternMatcher usePatternMatcher)
        {
            services.AddSingleton<IMappingMatcherProvider<int>, MappingMatcherProvider<int>>();
            services.AddSingleton<IPatternMatcherFactory<int>>(new DelegatePatternMatcherFactory<int>(usePatternMatcher));

            var conditionRegistry = new ConditionRegistry();
            services.AddSingleton<ConditionRegistry>(sp =>
            {
                conditionRegistry.ServiceProvider = sp;
                return conditionRegistry;
            });

            services.AddSingleton<SelectValue>(selectValueStrategy);
            return services;
        }
    }
}
