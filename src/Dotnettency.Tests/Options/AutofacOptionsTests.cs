using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dazinator.Extensions.Options.Updatable;
using Dotnettency.AspNetCore.Container;
using Dotnettency.Container;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dotnettency.Tests
{

    public class AutofacOptionsTests : XunitContextBase
    {

        private readonly ILoggerFactory _loggerFactory;

        public AutofacOptionsTests(ITestOutputHelper output) : base(output)
        {
            _loggerFactory = new LoggerFactory();

        }

        [Fact]
        public void CanResolveTOptionsFromChildContainer()
        {

            ServiceCollection services = new ServiceCollection();

            ContainerBuilder builder = new ContainerBuilder();
            builder.Populate(services);

            var rootContainer = builder.Build();
            var childContainer = rootContainer.BeginLifetimeScope((a) =>
            {

                var childServices = new ServiceCollection();
                childServices.AddOptions();
                childServices.Configure<MyOptions>((b) =>
                {
                    b.Foo = true;
                });
                a.Populate(childServices);
            });

            var childSp = new AutofacServiceProvider(childContainer);
            IOptions<MyOptions> options = childSp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Foo);

        }

        [Fact]
        public void CanResolveTOptionsFromRootContainer()
        {

            ServiceCollection services = new ServiceCollection();

            ContainerBuilder builder = new ContainerBuilder();
            services.AddOptions();
            services.Configure<MyOptions>((b) =>
            {
                b.Foo = true;
            });

            builder.Populate(services);

            var rootContainer = builder.Build();

            var rootSp = new AutofacServiceProvider(rootContainer);
            IOptions<MyOptions> options = rootSp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Foo);

        }


        [Fact]
        public async Task CanResolveTOptionsFromTenantRequestContainer()
        {
            ILogger<AutofacOptionsTests> logger = _loggerFactory.CreateLogger<AutofacOptionsTests>();

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
            IServiceProvider serviceProvider = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .Identify(async () =>
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
                    .InitialiseShell(tenantId =>
                    {
                        if (tenantId.Uri.Scheme != "unittest")
                        {
                            throw new ArgumentException();
                        }

                        return new TenantShell<Tenant>(new Tenant() { Name = tenantId.Uri.Host }, tenantId);
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
                        containerBuilder.SetDefaultServices(services);
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
                        .AutofacAsync(async (tenant, tenantServices) =>
                        {
                            var tenantConfig = await tenant.GetConfiguration();
                            var section = tenantConfig.GetSection("thing");
                            tenantServices.Configure<MyOptions>(section);

                            //  tenantServices.AddOptions();
                            //// override singleton options already registered, so we can have singleton at tenant level.
                            //  services.RemoveAll(typeof(IOptions<>));
                            //  services.Add(ServiceDescriptor.Singleton(typeof(IOptions<>), typeof(OptionsManager<>)));

                            //services.TryAdd(ServiceDescriptor.Scoped(typeof(IOptionsSnapshot<>), typeof(OptionsManager<>)));

                            //  services.RemoveAll(typeof(IOptionsMonitor<>));
                            //  services.Add(ServiceDescriptor.Singleton(typeof(IOptionsMonitor<>), typeof(OptionsMonitor<>)));
                            //services.TryAdd(ServiceDescriptor.Transient(typeof(IOptionsFactory<>), typeof(OptionsFactory<>)));

                            //services.RemoveAll(typeof(IOptionsMonitorCache<>));
                            //services.Add(ServiceDescriptor.Singleton(typeof(IOptionsMonitorCache<>), typeof(Dotnettency.Tests.Options.OptionsCache<>)));
                            // return services;

                            // tenantServices.AddOptions();
                            // both tenants should have tenant name options set.

                            var sp = tenantServices.BuildServiceProvider();
                            var options = sp.GetRequiredService<IOptions<MyOptions>>();

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
            ILogger<AutofacOptionsTests> logger = _loggerFactory.CreateLogger<AutofacOptionsTests>();

            IServiceCollection services = new ServiceCollection() as IServiceCollection;
            services.AddLogging();

            services.AddOptions();
            services.AddOptionsManagerBackedByMonitorCache();

            //var configBuilder = new ConfigurationBuilder();
            //configBuilder.SetBasePath(Environment.CurrentDirectory);
            //var inMemoryConfig = new Dictionary<string, string>();
            //inMemoryConfig["TenantName"] = "bar";
            //configBuilder.AddInMemoryCollection(inMemoryConfig);
            //var config = configBuilder.Build();
            //services.Configure<MyOptions>(config);

            bool isTenantA = true;
            IServiceProvider serviceProvider = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .Identify(async () =>
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
                    .InitialiseShell(tenantId =>
                    {
                        if (tenantId.Uri.Scheme != "unittest")
                        {
                            throw new ArgumentException();
                        }

                        return new TenantShell<Tenant>(new Tenant() { Name = tenantId.Uri.Host }, tenantId);
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
                        containerBuilder.SetDefaultServices(services);
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
                        containerBuilder.AutofacAsync(async (tenant, tenantServices) =>
                        {

                            //tenantServices.RemoveAll(typeof(IOptionsMonitor<>));
                            //tenantServices.RemoveAll(typeof(IOptionsMonitorCache<>));
                            //tenantServices.RemoveAll(typeof(IOptions<>));
                            //tenantServices.RemoveAll(typeof(IOptionsFactory<>));
                            //tenantServices.RemoveAll(typeof(IOptionsSnapshot<>));

                            tenantServices.AddOptions();
                            tenantServices.AddOptionsManagerBackedByMonitorCache();

                            var tenantConfig = await tenant.GetConfiguration();
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

        [Fact]
        public async Task CanGetReloadedOptionsAfterConfigChanged()
        {
            ILogger<AutofacOptionsTests> logger = _loggerFactory.CreateLogger<AutofacOptionsTests>();

            IServiceCollection services = new ServiceCollection() as IServiceCollection;
            services.AddLogging();

            services.AddOptions();
            services.AddOptionsManagerBackedByMonitorCache();

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
            IServiceProvider serviceProvider = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .Identify(async () =>
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
                   .InitialiseShell(tenantId =>
                    {
                        if (tenantId.Uri.Scheme != "unittest")
                        {
                            throw new ArgumentException();
                        }

                        return new TenantShell<Tenant>(new Tenant() { Name = tenantId.Uri.Host }, tenantId);
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
                        containerBuilder.SetDefaultServices(services);
                        // Extension methods available here for supported containers. We are using structuremap..
                        // We are using an overload that allows us to configure structuremap with familiar IServiceCollection.
                        containerBuilder.AutofacAsync(async (tenant, tenantServices) =>
                        {

                            //tenantServices.RemoveAll(typeof(IOptionsMonitor<>));
                            //tenantServices.RemoveAll(typeof(IOptionsMonitorCache<>));
                            //tenantServices.RemoveAll(typeof(IOptions<>));
                            //tenantServices.RemoveAll(typeof(IOptionsFactory<>));
                            //tenantServices.RemoveAll(typeof(IOptionsSnapshot<>));

                            tenantServices.AddOptions();
                            tenantServices.AddOptionsManagerBackedByMonitorCache();

                            var tenantConfig = await tenant.GetConfiguration();
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
                            services.AddJsonUpdatableOptions<MyOptions>(section.Path, jsonProvider);

                        });
                    });
            });

            IServiceScopeFactory scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

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

                var tenantConfig = scopeTenantBNextRequestAfterUpdate.ServiceProvider.GetRequiredService<Task<IConfiguration>>();
                var tConfig = tenantConfig.Result;


                registeredoptions.OnChange<MyOptions>((a) =>
                {
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




