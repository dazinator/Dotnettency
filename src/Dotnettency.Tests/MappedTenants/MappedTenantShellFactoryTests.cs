using Dotnettency.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dotnettency.Tests
{
    public partial class MappedTenantShellFactoryTests : XunitContextBase
    {

        private readonly ILoggerFactory _loggerFactory;

        public MappedTenantShellFactoryTests(ITestOutputHelper output) : base(output)
        {
            _loggerFactory = new LoggerFactory();
        }

        [Fact]
        public async Task Can_Get_IdentifiedTenant()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>))
                       .SetMockHttpContextProvider(new System.Uri("http://t1.foo.com"))
                       .Map<int>((a) =>
                       {
                           a.SelectRequestHost();
                       })
                       .Get((key) =>
                       {
                           var tenant = new Tenant() { Id = key, Name = "Default" };
                           return Task.FromResult(tenant);
                       });
            });


            services.Configure<TenantMappingOptions<int>>((b) =>
                {
                    b.Mappings = new TenantMapping<int>[] {
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
        public async Task Cannot_Get_UnidentifiedTenant()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>))
                       .SetMockHttpContextProvider(new System.Uri("http://unknown.foo.com"))
                       .Map<int>((m) =>
                       {
                           m.SelectRequestHost()
                            .WithMapping((tenants) =>
                            {
                                tenants.Add(1, "t1.foo.com", "t1.foo.uk");
                            })
                            .UsingDotNetGlobPatternMatching();
                       })
                       .Get((key) =>
                       {
                           if (key == 1)
                           {
                               return Task.FromResult(new Tenant() { Id = key, Name = "Test Tenant" });
                           }
                           return Task.FromResult<Tenant>(null); // key does not match a recognised tenant.
                       });
            });

            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<Task<Tenant>>();

            var tenant = await sut;
            Assert.Null(tenant);

            var tenantShellAccessor = sp.GetRequiredService<ITenantShellAccessor<Tenant>>();
            Assert.Null(await tenantShellAccessor.CurrentTenantShell.Value);

        }

        [Fact]
        public async Task Can_Get_IdentifiedTenant_WithInjectedServices()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>))
                       .SetMockHttpContextProvider(new System.Uri("http://t1.foo.com"))
                       .Identify(() =>
                       {
                           return Task.FromResult(new TenantIdentifier(new System.Uri("key://int32/1")));
                       })
                       .Get<TenantFactoryWithDependency>(ServiceLifetime.Scoped);
                //.InitialiseTenant<TestInjectedMappedTenantShellFactory>();
            });


            services.Configure<TenantMappingOptions<int>>((b) =>
            {
                b.Mappings = new TenantMapping<int>[] {
                        new TenantMapping<int>()
                        {
                            Key = 1,
                            Patterns = new string[]
                            {
                                "*.foo.com", // requests matching either of these url patterns should resolve to tenant key 1.                               
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
        public async Task Can_Used_Different_Factory_Based_On_Condition()
        {
            // Demonstrates a strategy to initialise a tenant.
            // When the system is in System Setup Mode, we always return the special System Setup tenant.
            // However once system setup is complete, we start returning actual tenants from the database etc.
            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();

            services.AddOptions<SystemSetupOptions>().Configure((a) => a.IsSystemSetupComplete = false);

            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>))
                       .SetMockHttpContextProvider(new System.Uri("http://t1.foo.com"))
                        .Map<int>((m) =>
                        {
                            m //.IdentifyWith<SystemSetupIdentifierFactory>() //override the default identifier factory with one that can use our SystemSetupOptions to map to a special -1 tenant when in setup mode.
                             .SelectRequestHost()
                             .WithMapping((tenants) =>
                             {
                                 tenants.Add(1, new string[] { "*.foo.com" }, "IsSetupComplete", true);
                                 tenants.Add(-1, "systemsetup", new string[] { "**" });
                             })
                             .UsingDotNetGlobPatternMatching()
                             .RegisterConditions(r =>
                             {
                                 r.Add("IsSetupComplete", sp =>
                                 {
                                     var options = sp.GetRequiredService<IOptionsSnapshot<SystemSetupOptions>>();
                                     return options.Value.IsSystemSetupComplete;
                                 });
                             });
                        })
                         .Get((key) =>
                         {
                             var tenant = new Tenant() { Id = key, Name = "System Setup" };
                             Assert.NotEqual(-1, tenant.Id); // shouldn't ever be -1, as in that case we use a named factory get.                                
                             return Task.FromResult(tenant);
                         })
                         .NamedGet(r => r.Add<SystemSetupTenantFactory>(ServiceLifetime.Scoped, "systemsetup"));

            });          

            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<Task<Tenant>>();

            var tenant = await sut;
            Assert.NotNull(tenant);
            Assert.Equal(-1, tenant.Id);
            Assert.True(tenant.IsSystemSetup);
        }

    }

    public class SimpleTenantFactory : TenantFactory<Tenant, int>
    {
        public override Task<Tenant> GetTenant(int value)
        {
            return Task.FromResult(new Tenant() { Id = value, Name = "Simple" });
        }
    }

    public class SystemSetupTenantFactory : TenantFactory<Tenant, int>
    {
        public override Task<Tenant> GetTenant(int value)
        {
            return Task.FromResult(new Tenant() { Id = value, Name = "System Setup", IsSystemSetup = true });
        }
    }
}




