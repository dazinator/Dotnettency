using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dotnettency.Tests
{
    public class MappedRequestIdentifierFactoryTests : XunitContextBase
    {

        private readonly ILoggerFactory _loggerFactory;

        public MappedRequestIdentifierFactoryTests(ITestOutputHelper output) : base(output)
        {
            _loggerFactory = new LoggerFactory();
        }

        [Fact]
        public async Task Can_Get_Mapped_TenantIdentifier()
        {

            bool isFirstRequest = true;

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();

            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetMockHttpContextProvider(() =>
                {
                    if (isFirstRequest)
                    {
                        return new System.Uri("http://t1.foo.com");
                    }
                    else
                    {
                        return new System.Uri("https://t1.foo.uk?bar=1");
                    }

                })
                .SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>))
                .MapFromHttpContext<int>((m) =>
                {
                    m.MapRequestHost()
                     .WithMapping((tenants) =>
                     {
                         tenants.Add(1, "t1.foo.com", "t1.foo.uk");
                     })
                     .UsingDotNetGlobPatternMatching();
                });
            });

            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<ITenantIdentifierFactory<Tenant>>();

            var identfier = await sut.IdentifyTenant();
            Assert.NotNull(identfier);
            Assert.Equal("/1", identfier.Uri.PathAndQuery);

            // Do another request with a varying url that also maps to the same tenant, and verify that the identifier returned is equal.
            isFirstRequest = false;
            var nextIdentifier = await sut.IdentifyTenant();
            Assert.Equal(nextIdentifier, identfier);

        }

        [Fact]
        public async Task Can_Get_Mapped_TenantIdentifier_FromConfig()
        {
            var currentDir = Environment.CurrentDirectory;
            // var currentLocation = Assembly.GetExecutingAssembly().Location;
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.SetBasePath(currentDir)
                         .AddJsonFile("tenant-mappings.json")
                         .Build();

            bool isFirstRequest = true;

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.Configure<TenantMappingOptions<int>>(config);

            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetMockHttpContextProvider(() =>
                {
                    if (isFirstRequest)
                    {
                        return new System.Uri("http://t1.foo.com");
                    }
                    else
                    {
                        return new System.Uri("https://t1.foo.uk?bar=1");
                    }

                })
                .SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>))
                .MapFromHttpContext<int>((m) =>
                {
                    m.MapRequestHost()
                     .UsingDotNetGlobPatternMatching();
                });
            });

            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<ITenantIdentifierFactory<Tenant>>();

            var identfier = await sut.IdentifyTenant();
            Assert.NotNull(identfier);
            Assert.Equal("/1", identfier.Uri.PathAndQuery);

            // Do another request with a varying url that also maps to the same tenant, and verify that the identifier returned is equal.
            isFirstRequest = false;
            var nextIdentifier = await sut.IdentifyTenant();
            Assert.Equal(nextIdentifier, identfier);

        }

        [Fact]
        public async Task Can_Get_ConditionallyMapped_TenantIdentifier_FromConfig()
        {
            var currentDir = Environment.CurrentDirectory;
            // var currentLocation = Assembly.GetExecutingAssembly().Location;
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.SetBasePath(currentDir)
                         .AddJsonFile("tenant-mappings-conditions.json")
                         .Build();

            bool isFirstRequest = true;

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.Configure<TenantMappingOptions<int>>(config);


            bool setupComplete = false;

            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetMockHttpContextProvider(() =>
                {
                    if (isFirstRequest)
                    {
                        return new System.Uri("http://t1.foo.com");
                    }
                    else
                    {
                        return new System.Uri("https://t1.foo.uk?bar=1");
                    }

                })
                .SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>))
                .MapFromHttpContext<int>((m) =>
                {
                    m.MapRequestHost()
                    .WithMapping((a) =>
                    {
                        a.Add(key: -1,
                              patterns: new string[] { "**" },
                              conditionName: "IsSetupComplete", false);

                        a.Add(key: 1, 
                              patterns: new string[] { "*.foo.com", "*.foo.uk" });
                    })
                    .RegisterConditions((c) =>
                    {
                        c.Add("IsSetupComplete", (sp) =>
                        {
                            return setupComplete;
                        });
                    })
                    .NamedFactories(a =>
                    {

                    })
                     .UsingDotNetGlobPatternMatching();
                });
            });

            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<ITenantIdentifierFactory<Tenant>>();

            var identfier = await sut.IdentifyTenant();
            Assert.NotNull(identfier);
            Assert.Equal("/-1", identfier.Uri.PathAndQuery);

            // Do another request, this time the condition evaluates to true, disabling the first mapping so we map to a different key now.
            isFirstRequest = false;
            setupComplete = true;

            var nextIdentifier = await sut.IdentifyTenant();
            Assert.Equal("/1", nextIdentifier.Uri.PathAndQuery);

        }

    }
}




