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
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.SetGenericOptionsProvider(typeof(OptionsMonitorOptionsProvider<>))
                       .SetMockHttpContextProvider(new System.Uri("http://t1.foo.com"))
                       .Identify(() =>
                       {
                           // note: we append a key "/1" to the Uri here which is used as an "identifier" for the tenant.
                           // this additional key information can then be leveraged by the tenant shell factory, 
                           // - it the tenant with that id, and this is what
                           // our custom / options based mapper will do for us usually by default - but we want to eliminate that from 
                           // this unit test so we are appending this id manually as if it was a result of doing the mapping.
                           return Task.FromResult(new KeyTenantIdentifier<int>(1).AsTenantIdentifier());

                       });
                // .InitialiseTenant<TestMappedTenantShellFactory>();
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
                       .MapFromHttpContext<int>((m) =>
                       {
                           m.MapRequestHost()
                            .WithMapping((tenants) =>
                            {
                                tenants.Add(1, "t1.foo.com", "t1.foo.uk");
                            })
                            .UsingDotNetGlobPatternMatching()
                            .Factory((key) =>
                            {
                                if (key == 1)
                                {
                                    return Task.FromResult(new Tenant() { Id = key, Name = "Test Tenant" });
                                }
                                return Task.FromResult<Tenant>(null); // key does not match a recognised tenant.
                            });
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
                       });
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
        public async Task Can_Override_IdentifiedTenant()
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
                        .MapFromHttpContext<int>((m) =>
                        {
                            m //.IdentifyWith<SystemSetupIdentifierFactory>() //override the default identifier factory with one that can use our SystemSetupOptions to map to a special -1 tenant when in setup mode.
                             .MapRequestHost()
                             .WithMapping((tenants) =>
                             {
                                 tenants.Add(1, "t1.foo.com", "t1.foo.uk");
                             })
                             .UsingDotNetGlobPatternMatching()
                             //.OverrideInitialise((key) =>
                             //{
                             //    if (key == -1)
                             //    {
                             //        // alternative implementation of MappedTenantShellFactory with no dependencies injected that touch unconfigured services such as database etc.
                             //        // This implemenation, because we are in System Setup mode, will just construct a tenant without querying database etc, and sets special flag.
                             //        return typeof(SystemSetupMappedTenantShellFactory);
                             //    }
                             //    return null; // don't override - default Initialise will be used.
                             //})
                             .Factory((key) =>
                             {
                                 var tenant = new Tenant() { Id = key, Name = "Test" };
                                 Assert.NotEqual(-1, tenant.Id); // shouldn't ever be -1, as we are overriden by above in that case.                                
                                 return Task.FromResult(tenant);
                             });
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
                                "*.foo.com", // requests matching either of these url patterns should resolve to tenant key 1.                               
                            }
                       }
                    };
            });

            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<Task<Tenant>>();

            var tenant = await sut;
            Assert.NotNull(tenant);
            Assert.Equal(-1, tenant.Id);
            Assert.True(tenant.IsSystemSetup);
        }


        [Fact]
        public async Task Can_Get_Tenant_With_Factory()
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
                        .MapFromHttpContext<int>((m) =>
                        {
                            m //.IdentifyWith<SystemSetupIdentifierFactory>() //override the default identifier factory with one that can use our SystemSetupOptions to map to a special -1 tenant when in setup mode.
                             .MapRequestHost()
                             .WithMapping((tenants) =>
                             {
                                 tenants.Add(1, "t1.foo.com", "t1.foo.uk");
                                 tenants.Add(2, factoryName: nameof(SimpleFactory), "t2.bar.com", "t2.bar.uk");
                             })
                             .UsingDotNetGlobPatternMatching()
                             .NamedFactories(factories =>
                             {
                                 factories.AddSingleton<SimpleFactory>(nameof(SimpleFactory));
                             })
                             .Factory((key) =>
                             {
                                 var tenant = new Tenant() { Id = key, Name = "Default" };
                                 Assert.NotEqual(-1, tenant.Id); // shouldn't ever be -1, as we are overriden by above in that case.                                
                                 return Task.FromResult(tenant);
                             });
                        });

            });


            var sp = services.BuildServiceProvider();
            var sut = sp.GetRequiredService<Task<Tenant>>();

            var tenant = await sut;
            Assert.NotNull(tenant);
            Assert.Equal(-1, tenant.Id);
            Assert.True(tenant.IsSystemSetup);
        }


    }

    public class SimpleFactory : TenantFactory<Tenant, int>
    {
        public override Task<Tenant> GetTenant(int value)
        {
            return Task.FromResult(new Tenant() { Id = value, Name = "Simple" });
        }
    }
}




