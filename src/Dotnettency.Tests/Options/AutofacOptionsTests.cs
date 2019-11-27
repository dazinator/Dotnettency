using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dotnettency.AspNetCore.Container;
using Dotnettency.Container;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Dotnettency.Tests
{


    public class AutofacOptionsTests
    {

        private readonly ILoggerFactory _loggerFactory;


        public AutofacOptionsTests()
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
    }
}




