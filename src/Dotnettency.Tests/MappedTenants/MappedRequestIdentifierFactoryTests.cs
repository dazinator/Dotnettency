using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dazinator.Extensions.Options.Updatable;
using Dotnettency.Container;
using Dotnettency.Extensions.MappedTenants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dotnettency.Tests
{

    public class TestHttpContextProvider : IHttpContextProvider
    {
        public TestHttpContextProvider(HttpContextBase httpContextBase)
        {
            CurrentHttpContext = httpContextBase;
        }

        public HttpContextBase CurrentHttpContext { get; set; }

        public HttpContextBase GetCurrent()
        {
            return CurrentHttpContext;

        }


    }

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

            var request = new Mock<RequestBase>(MockBehavior.Strict);
            request.Setup(r => r.GetUri()).Returns(new Uri("http://t1.foo.com:80"));

            var context = new Mock<HttpContextBase>();
            context.SetupGet(c => c.Request).Returns(request.Object);

            var testHttpContextProvider = new TestHttpContextProvider(context.Object);

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetHttpContextProvider(testHttpContextProvider)
                       .IdentifyTenantsUsingRequestAuthorityMapping<Tenant, TenantMappingOptions<int>, int>();
            });


            services.Configure<TenantMappingOptions<int>>((b) =>
                {
                    b.TenantMappings = new TenantMapping<int>[] {
                        new TenantMapping<int>()
                        {
                            Key = 1,
                            Patterns = new string[]
                            {
                                "t1.foo.com",
                                "t1.foo.uk"
                            }
                       }
                    };
                });

            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<ITenantIdentifierFactory<Tenant>>();

            var identfier = await sut.IdentifyTenant();
            Assert.NotNull(identfier);
            Assert.Equal("/1", identfier.Uri.PathAndQuery);

        }

    }
}




