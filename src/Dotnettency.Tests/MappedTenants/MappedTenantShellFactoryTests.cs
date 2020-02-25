using Dotnettency.Extensions.MappedTenants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dotnettency.Tests
{
    public class MappedTenantShellFactoryTests : XunitContextBase
    {

        private readonly ILoggerFactory _loggerFactory;

        public MappedTenantShellFactoryTests(ITestOutputHelper output) : base(output)
        {
            _loggerFactory = new LoggerFactory();
        }

        [Fact]
        public async Task Can_Get_Tenant()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetMockHttpContextProvider(new System.Uri("http://t1.foo.com"))
                       .IdentifyTenantTask(() =>
                       {
                           // note: we append a key "/1" to the Uri here which is used as an "identifier" for the tenant.
                           // this additional key information can then be leveraged by the tenant shell factory, 
                           // - it the tenant with that id, and this is what
                           // our custom / options based mapper will do for us usually by default - but we want to eliminate that from 
                           // this unit test so we are appending this id manually as if it was a result of doing the mapping.
                           return Task.FromResult(new TenantIdentifier(new System.Uri("key://int32/1")));

                       })
                       .InitialiseTenant<TestMappedTenantShellFactory>();
            });


            services.Configure<TenantMappingOptions<int>>((b) =>
                {
                    b.TenantMappings = new TenantMapping<int>[] {
                        new TenantMapping<int>()
                        {
                            Key = 1,
                            Patterns = new string[]
                            {
                                "t1.foo.com", // requests matching either of these url patterns should resolve to tenant key 1.
                                "t1.foo.uk"
                            }
                       }
                    };
                });

            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<Task<Tenant>>();

            var tenant = await sut;
            Assert.NotNull(tenant);
            Assert.Equal(1, tenant.Id);

        }

        [Fact]
        public async Task Can_Get_DefaultTenant()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetMockHttpContextProvider(new System.Uri("http://unknown.foo.com"))                       
                       .MapFromHttpContext<int>((m) =>
                       {
                           m.MapRequestHost()
                            .ToTenants((tenants) =>
                            {
                                tenants.Add(1, "t1.foo.com", "t1.foo.uk");
                            })
                            .UsingDotNetGlobPatternMatching();
                       });

                       //.IdentifyTenantsUsingRequestAuthorityMapping<Tenant, int, TenantMappingOptions<int>>()
                       //.InitialiseTenant<TestMappedTenantShellFactory>();
            });


            services.Configure<TenantMappingOptions<int>>((b) =>
            {
                b.TenantMappings = new TenantMapping<int>[] {
                        new TenantMapping<int>()
                        {
                            Key = 1,
                            Patterns = new string[]
                            {
                                "t1.foo.com", // requests matching either of these url patterns should resolve to tenant key 1.
                                "t1.foo.uk"
                            }
                       }
                    };
            });

            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<Task<Tenant>>();

            var tenant = await sut;
            Assert.Null(tenant);

        }

    }
}




