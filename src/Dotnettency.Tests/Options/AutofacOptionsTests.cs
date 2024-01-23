using Dazinator.Extensions.Options.Updatable;
using Dotnettency.Container;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Threading.Tasks;
using Dotnettency.Container.Native;
using Xunit;
using Xunit.Abstractions;

namespace Dotnettency.Tests
{

    public class NativeOptionsTests : XunitContextBase
    {

        private readonly ILoggerFactory _loggerFactory;

        public NativeOptionsTests(ITestOutputHelper output) : base(output)
        {
            _loggerFactory = new LoggerFactory();

        }

        [Fact]
        public void CanResolveTOptionsFromChildContainer()
        {

            var services = new ServiceCollection();
            services.AddLogging();
            var rootSp = services.BuildServiceProvider();
            
            //services.AddChildContainers();
            var logger = rootSp.GetRequiredService<ILogger<NativeTenantContainerAdaptor>>();
            var rootContainerAdaptor = new NativeTenantContainerAdaptor(logger, rootSp, services, ContainerRole.Root, "Root");
               
            var adapted = rootContainerAdaptor.GetRequiredService<ITenantContainerAdaptor>();
            
            var childContainer = adapted.CreateChild("a",(a) =>
            {
                a.AddOptions();
                a.AutoDuplicateSingletons((child) =>
                {
                    child.Configure<MyOptions>((b) =>
                    {
                        b.Foo = true;
                    });
                });
            });
          
            IOptions<MyOptions> options = childContainer.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Foo);
            
            var rootOptions = rootContainerAdaptor.GetRequiredService<IOptions<MyOptions>>();
            Assert.False(rootOptions.Value?.Foo);
        }

        [Fact]
        public void CanResolveTOptionsFromRootContainer()
        {

            var services = new ServiceCollection();
     
            services.AddOptions();
            services.Configure<MyOptions>((b) =>
            {
                b.Foo = true;
            });
            
            var rootSp = services.BuildServiceProvider();
            IOptions<MyOptions> options = rootSp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Foo);

        }


        [Fact]
        public async Task CanResolveTOptionsFromTenantRequestContainer()
        {
            ILogger<NativeOptionsTests> logger = _loggerFactory.CreateLogger<NativeOptionsTests>();

            //NOTE add logging and options were added to default services..
            IServiceCollection services = new ServiceCollection() as IServiceCollection;
            services.AddLogging();
            services.AddOptions();

            //var configBuilder = new ConfigurationBuilder();
            //configBuilder.SetBasePath(Environment.CurrentDirectory);
            //var inMemoryConfig = new Dictionary<string, string>();
            //inMemoryConfig["TenantName"] = "bar";
            //configBuilder.AddInMemoryCollection(inMemoryConfig);
            //var config = configBuilder.Build();
            //services.Configure<MyOptions>(config);

            bool isTenantA = true;
            services = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .IdentifyTenantTask(async () =>
                    {

                        if (isTenantA)
                        {
                            return new TenantIdentifier(new Uri("unittest://tenanta"));
                        }
                        else
                        {
                            return new TenantIdentifier(new Uri("unittest://tenantb"));
                        }

                    })
                    .AddAspNetCore()
                    .InitialiseTenant(tenantId =>
                    {
                        if (tenantId.Uri.Scheme != "unittest")
                        {
                            throw new ArgumentException();
                        }

                        return new TenantShell<Tenant>(new Tenant() { Name = tenantId.Uri.Host });
                    })
                    .ConfigureTenantConfiguration((t, b) =>
                    {
                        var basePath = Environment.CurrentDirectory;
                        b.SetBasePath(basePath);
                        b.AddJsonFile($"appsettings-{t.Tenant.Name}.json");
                    })

                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                        // var defaultServices = services.RemoveOptions();
                        //  var defaultServices = services.RemoveOptions();
                       // containerBuilder.SetDefaultServices(services);
                        containerBuilder.Events((events) =>
                        {
                            // callback invoked after tenant container is created.
                            events.OnTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
                            {
                                Tenant tenant = await tenantResolver;
                            })
                            // callback invoked after a nested container is created for a tenant. i.e typically during a request.
                            .OnNestedTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
                            {
                                Tenant tenant = await tenantResolver;
                            });
                        })
                        // Extension methods available here for supported containers. We are using structuremap..
                        // We are using an overload that allows us to configure structuremap with familiar IServiceCollection.
                        .NativeAsync(async (tenant, tenantServices) =>
                        {
                            var tenantConfig = await tenant.GetConfigurationAsync();
                           

                            tenantServices.AutoDuplicateSingletons(c =>
                            {
                                c.AddSingleton<IConfiguration>(tenantConfig);
                                var section = tenantConfig.GetSection("thing");
                                c.Configure<MyOptions>(section);
                            });
                           
                            //return Task.CompletedTask;



                            // var sp = tenantServices.BuildServiceProvider();
                            //  var options = sp.GetRequiredService<IOptions<MyOptions>>();

                            //// but.. Tenant A and tenant B have different Foo settings.
                            //if (tenant.Tenant.Name == "tenanta")
                            //{
                            //    tenantServices.Configure<MyOptions>((a) => { a.Foo = true; });
                            //}
                            //else
                            //{
                            //    tenantServices.Configure<MyOptions>((a) => { a.Foo = false; });

                            //}
                        });
                    });
            });

            var serviceProvider = new DotnettencyServiceProviderFactory<Tenant>().CreateServiceProvider(services);
            IServiceScopeFactory scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scopeTenantA = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantA.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                IOptionsSnapshot<MyOptions> registeredoptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                var myOptions = registeredoptions.Value;

                Assert.True(myOptions.Foo);
            }

            isTenantA = false;

            using (var scopeTenantB = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantB.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                IOptionsSnapshot<MyOptions> registeredoptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                var myOptions = registeredoptions.Value;

                Assert.False(myOptions.Foo);
            }


        }

        [Fact]
        public async Task CanUpdateOptions()
        {
            ILogger<NativeOptionsTests> logger = _loggerFactory.CreateLogger<NativeOptionsTests>();

            var services = new ServiceCollection() as IServiceCollection;
            services.AddLogging();

            services.AddOptions();
          //  services.AddOptionsManagerBackedByMonitorCache();

            //var configBuilder = new ConfigurationBuilder();
            //configBuilder.SetBasePath(Environment.CurrentDirectory);
            //var inMemoryConfig = new Dictionary<string, string>();
            //inMemoryConfig["TenantName"] = "bar";
            //configBuilder.AddInMemoryCollection(inMemoryConfig);
            //var config = configBuilder.Build();
            //services.Configure<MyOptions>(config);

            bool isTenantA = true;
            services = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .IdentifyTenantTask(async () =>
                    {

                        if (isTenantA)
                        {
                            return new TenantIdentifier(new Uri("unittest://tenanta"));
                        }
                        else
                        {
                            return new TenantIdentifier(new Uri("unittest://tenantb"));
                        }

                    })
                    .AddAspNetCore()
                    .InitialiseTenant(tenantId =>
                    {
                        if (tenantId.Uri.Scheme != "unittest")
                        {
                            throw new ArgumentException();
                        }

                        return new TenantShell<Tenant>(new Tenant() { Name = tenantId.Uri.Host });
                    })
                    .ConfigureTenantConfiguration((t, b) =>
                    {
                        var basePath = Environment.CurrentDirectory;
                        var fileName = "scp-settings-tenant.json";
                        var fullPath = Path.Combine(basePath, fileName);
                        if (File.Exists(fileName))
                        {
                            // Ensure we don't have any settings file existing to begin with.
                            File.Delete(fileName);
                        }

                        //   var tenantConfig = t.GetConfiguration().Result;

                        b.SetBasePath(basePath);
                        b.AddJsonFile($"appsettings-{t.Tenant.Name}.json")
                         .AddJsonFile(fileName, true, true);
                    })

                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                        // var defaultServices = services.RemoveOptions();
                        //  var defaultServices = services.RemoveOptions();
                        //containerBuilder.SetDefaultServices(services);
                        //containerBuilder.Events((events) =>
                        //{
                        //    // callback invoked after tenant container is created.
                        //    events.OnTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
                        //    {
                        //        Tenant tenant = await tenantResolver;
                        //    })
                        //    // callback invoked after a nested container is created for a tenant. i.e typically during a request.
                        //    .OnNestedTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
                        //    {
                        //        Tenant tenant = await tenantResolver;
                        //    });
                        //})
                        // Extension methods available here for supported containers. We are using structuremap..
                        // We are using an overload that allows us to configure structuremap with familiar IServiceCollection.
                        containerBuilder.NativeAsync(async (tenant, tenantServices) =>
                        {

                            //tenantServices.RemoveAll(typeof(IOptionsMonitor<>));
                            //tenantServices.RemoveAll(typeof(IOptionsMonitorCache<>));
                            //tenantServices.RemoveAll(typeof(IOptions<>));
                            //tenantServices.RemoveAll(typeof(IOptionsFactory<>));
                            //tenantServices.RemoveAll(typeof(IOptionsSnapshot<>));

                            tenantServices.AddOptions();
                            //tenantServices.AddOptionsManagerBackedByMonitorCache();

                            var tenantConfig = await tenant.GetConfigurationAsync();
                            var section = tenantConfig.GetSection("thing");
                            tenantServices.Configure<MyOptions>(section);
                            //var name = Microsoft.Extensions.Options.Options.DefaultName;
                            //var source = new ConfigurationChangeTokenSource<MyOptions>(name, tenantConfig);
                            //tenantServices.AddSingleton<IOptionsChangeTokenSource<MyOptions>>(source);

                            var basePath = Environment.CurrentDirectory;
                            var jsonProvider = new FileJsonStreamProvider<MyOptions>(basePath, "scp-settings-tenant.json");
                            services.AddJsonUpdatableOptions<MyOptions>(section.Path, jsonProvider);

                        });
                    });
            });

            var serviceProvider = new DotnettencyServiceProviderFactory<Tenant>().CreateServiceProvider(services);
            IServiceScopeFactory scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scopeTenantA = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantA.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                IOptionsSnapshot<MyOptions> registeredoptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                var myOptions = registeredoptions.Value;

                Assert.True(myOptions.Foo);
            }

            isTenantA = false;

            using (var scopeTenantB = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantB.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                IOptionsSnapshot<MyOptions> registeredoptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                var myOptions = registeredoptions.Value;

                Assert.False(myOptions.Foo);
            }

            using (var scopeTenantBNextRequest = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantBNextRequest.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                var registeredoptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                var updater = requestContainer.GetRequiredService<IUpdatableOptions<MyOptions>>();
                var myOptions = registeredoptions.Value;
                updater.Update((a) => a.Foo = true);
                Assert.True(updater.Value.Foo);

            }



        }

        // I Suspect this test is failing because child container implementation is set to duplicate parent singletons to child container.
        [Fact]
        public async Task CanGetReloadedOptionsAfterConfigChanged()
        {
            ILogger<NativeOptionsTests> logger = _loggerFactory.CreateLogger<NativeOptionsTests>();

            var services = new ServiceCollection() as IServiceCollection;
            services.AddLogging();
            services.AddOptions();
           // services.AddOptionsManagerBackedByMonitorCache();

            //var configBuilder = new ConfigurationBuilder();
            //configBuilder.SetBasePath(Environment.CurrentDirectory);
            //var inMemoryConfig = new Dictionary<string, string>();
            //inMemoryConfig["TenantName"] = "bar";
            //configBuilder.AddInMemoryCollection(inMemoryConfig);
            //var config = configBuilder.Build();
            //services.Configure<MyOptions>(config);

            var sectionPath = "createdlater";
            string tenantBSettingsFileName = "";

            bool isTenantA = true;
            services = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .IdentifyTenantTask(async () =>
                    {

                        if (isTenantA)
                        {
                            return new TenantIdentifier(new Uri("unittest://tenanta"));
                        }
                        else
                        {
                            return new TenantIdentifier(new Uri("unittest://tenantb"));
                        }

                    })
                    .AddAspNetCore()
                    .InitialiseTenant(tenantId =>
                    {
                        if (tenantId.Uri.Scheme != "unittest")
                        {
                            throw new ArgumentException();
                        }

                        return new TenantShell<Tenant>(new Tenant() { Name = tenantId.Uri.Host });
                    })
                    .ConfigureTenantConfiguration((t, b) =>
                    {
                        var basePath = Environment.CurrentDirectory;
                        var fileName = $"updatable-settings-{t.Tenant.Name}.json";
                        var fullPath = Path.Combine(basePath, fileName);
                        if (File.Exists(fileName))
                        {
                            // Ensure we don't have any settings file existing to begin with.
                            File.Delete(fileName);
                        }

                        //   var tenantConfig = t.GetConfiguration().Result;

                        b.SetBasePath(basePath)
                         //   b.AddJsonFile($"appsettings-{t.Tenant.Name}.json")
                         .AddJsonFile(fileName, true, true);
                    })

                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                        // var defaultServices = services.RemoveOptions();
                        //  var defaultServices = services.RemoveOptions();
                      //  containerBuilder.SetDefaultServices(services);
                        // Extension methods available here for supported containers. We are using structuremap..
                        // We are using an overload that allows us to configure structuremap with familiar IServiceCollection.
                        containerBuilder.NativeAsync(async (tenant, tenantServices) =>
                        {

                            //tenantServices.RemoveAll(typeof(IOptionsMonitor<>));
                            //tenantServices.RemoveAll(typeof(IOptionsMonitorCache<>));
                            //tenantServices.RemoveAll(typeof(IOptions<>));
                            //tenantServices.RemoveAll(typeof(IOptionsFactory<>));
                            //tenantServices.RemoveAll(typeof(IOptionsSnapshot<>));

                            tenantServices.AddOptions();
                            //tenantServices.AddOptionsManagerBackedByMonitorCache();

                            var tenantConfig = await tenant.GetConfigurationAsync();
                            tenantServices.AddSingleton<IConfiguration>(tenantConfig);
                            var section = tenantConfig.GetSection(sectionPath);
                            tenantServices.Configure<MyOptions>(section);
                            //,

                            //    (a) =>
                            //    {
                            //        if (!section.Exists())
                            //        {
                            //            section = tenantConfig.GetSection(sectionPath);  

                            //        }
                            //    });
                            //var name = Microsoft.Extensions.Options.Options.DefaultName;
                            //var source = new ConfigurationChangeTokenSource<MyOptions>(name, tenantConfig);
                            //tenantServices.AddSingleton<IOptionsChangeTokenSource<MyOptions>>(source);

                            var basePath = Environment.CurrentDirectory;
                            var fileName = $"updatable-settings-{tenant.Tenant.Name}.json";

                            var jsonProvider = new FileJsonStreamProvider<MyOptions>(basePath, fileName);
                            tenantServices.AddJsonUpdatableOptions<MyOptions>(section.Path, jsonProvider);

                        });
                    });
            });

            var rootSp = services.BuildServiceProvider();
            //services.AddChildContainers();
            var rootContainerAdaptorLogger = rootSp.GetRequiredService<ILogger<NativeTenantContainerAdaptor>>();
            var rootContainerAdaptor = new NativeTenantContainerAdaptor(rootContainerAdaptorLogger, rootSp, services, ContainerRole.Root, "Root");
            
            //var serviceProvider = new DotnettencyServiceProviderFactory<Tenant>().CreateServiceProvider(services);

           // var serviceProvider = spFactory.CreateServiceProvider();
            
            IServiceScopeFactory scopeFactory = rootContainerAdaptor.GetRequiredService<IServiceScopeFactory>();

            using (var scopeTenantA = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantA.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                IOptionsSnapshot<MyOptions> registeredoptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                var myOptions = registeredoptions.Value;

                Assert.False(myOptions.Foo);
            }

            isTenantA = false;

            using (var scopeTenantB = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantB.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                IOptionsSnapshot<MyOptions> registeredoptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                var myOptions = registeredoptions.Value;

                Assert.False(myOptions.Foo);
            }

            using (var scopeTenantBNextRequest = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantBNextRequest.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                var registeredoptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                var updater = requestContainer.GetRequiredService<IUpdatableOptions<MyOptions>>();
                // var myOptions = registeredoptions.Value;
                updater.Update((a) => a.Foo = true);
                Assert.True(updater.Value.Foo);

            }

            using (var scopeTenantBNextRequestAfterUpdate = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantBNextRequestAfterUpdate.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                var snapshotOptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                Assert.True(snapshotOptions.Value.Foo);

                var registeredoptions = requestContainer.GetRequiredService<IOptionsMonitor<MyOptions>>();
                var myOptions = registeredoptions.CurrentValue;
                Assert.True(myOptions.Foo);

                var updater = requestContainer.GetRequiredService<IUpdatableOptions<MyOptions>>();
                Console.WriteLine("Writing options to file..");
                updater.Update((a) => a.Foo = false);
                Assert.False(updater.Value.Foo);

            }

            using (var scopeTenantBNextRequestAfterUpdate = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantBNextRequestAfterUpdate.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                var registeredoptions = requestContainer.GetRequiredService<IOptionsMonitor<MyOptions>>();

                var myOptions = registeredoptions.CurrentValue;
                Assert.False(myOptions.Foo);

                var snapshotOptions = requestContainer.GetRequiredService<IOptionsSnapshot<MyOptions>>();
                Assert.False(snapshotOptions.Value.Foo);

                // manually change the underlying file.
                var tenantAccessor = requestContainer.GetRequiredService<ITenantAccessor<Tenant>>();
                var currentTenantName = tenantAccessor.CurrentTenant.Value.Result.Name;
                var settingsFileName = $"updatable-settings-{currentTenantName}.json";

                var tenantContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                //var sp = tenantContainer.
                var tConfig = tenantContainer.GetRequiredService<IConfiguration>();
              //  var tConfig = tenantConfig.Result;


                registeredoptions.OnChange<MyOptions>((a) => {
                    Console.WriteLine("Options Montior On Change..");

                });

                ChangeToken.OnChange(() => tConfig.GetReloadToken(), () =>
                {
                    Console.WriteLine("Config reload token change callback fired..");
                });

                var val = tConfig["createdlater:foo"];
                Assert.Null(val);

                var settingsFilePath = Path.Combine(Environment.CurrentDirectory, settingsFileName);
                Console.WriteLine("Writing change to file..");
                File.WriteAllText(settingsFilePath, @"{ ""createdlater"": { ""foo"": true } }");

                await Task.Delay(1000);

                val = tConfig["createdlater:foo"];
                Assert.Equal("True", val);

                var testOptions = new MyOptions();
                var testSection = tConfig.GetSection("createdlater");
                testSection.Bind(testOptions);
                Assert.True(testOptions.Foo);

                // await Task.Delay(10000);

                myOptions = registeredoptions.CurrentValue;
                Assert.True(myOptions.Foo);

                Console.WriteLine("Writing change to file..");
                File.WriteAllText(settingsFilePath, @"{ ""createdlater"": { ""foo"": false } }");
                await Task.Delay(1000);

                myOptions = registeredoptions.CurrentValue;
                Assert.False(myOptions.Foo);

                Console.WriteLine("Writing change to file..");
                File.WriteAllText(settingsFilePath, @"{ ""createdlater"": { ""foo"": true } }");
                await Task.Delay(1000);

            }


            using (var scopeTenantBNextRequestAfterUpdate = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantBNextRequestAfterUpdate.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                var registeredoptions = requestContainer.GetRequiredService<IOptionsMonitor<MyOptions>>();

                var myOptions = registeredoptions.CurrentValue;
                Assert.True(myOptions.Foo);

            }

            isTenantA = true;
            using (var scopeTenantANextRequestAfterUpdate = scopeFactory.CreateScope())
            {
                ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scopeTenantANextRequestAfterUpdate.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
                var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

                var registeredoptions = requestContainer.GetRequiredService<IOptionsMonitor<MyOptions>>();

                var myOptions = registeredoptions.CurrentValue;
                Assert.False(myOptions.Foo);
            }

            //using (var scopeTenantBNextRequestAfterUpdate = scopeFactory.CreateScope())
            //{
            //    var tenantRequestContainerAccessor = scopeTenantBNextRequestAfterUpdate.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
            //    var requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

            //    var tenantConfig = scopeTenantBNextRequestAfterUpdate.ServiceProvider.GetRequiredService<Task<IConfiguration>>();
            //    var tConfig = tenantConfig.Result;
            //    var val = tConfig["thing:foo"];
            //    Assert.Equal("true", val);

            //    var registeredoptions = requestContainer.GetRequiredService<IOptionsMonitor<MyOptions>>();
            //    var myOptions = registeredoptions.CurrentValue;

            //    Assert.True(myOptions.Foo);


            //}


        }

    }
}




