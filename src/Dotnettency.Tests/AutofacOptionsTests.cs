using Autofac;
using Autofac.Extensions.DependencyInjection;
using Dotnettency.AspNetCore.Container;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
            var childContainer = rootContainer.BeginLifetimeScope((a)=> {

                var childServices = new ServiceCollection();
                childServices.AddOptions();
                childServices.Configure<MyOptions>((b) =>
                {
                    b.Prop = true;
                });
                a.Populate(childServices);
            });

            var childSp = new AutofacServiceProvider(childContainer);           
            IOptions<MyOptions> options = childSp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Prop);

        }

        [Fact]
        public void CanResolveTOptionsFromRootContainer()
        {

            ServiceCollection services = new ServiceCollection();

            ContainerBuilder builder = new ContainerBuilder();
            services.AddOptions();
            services.Configure<MyOptions>((b) =>
            {
                b.Prop = true;
            });

            builder.Populate(services);

            var rootContainer = builder.Build();           

            var rootSp = new AutofacServiceProvider(rootContainer);
            IOptions<MyOptions> options = rootSp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Prop);

        }


        [Fact]
        public async Task CanResolveTOptionsFromTenantRequestContainer()
        {
            ILogger<AutofacOptionsTests> logger = _loggerFactory.CreateLogger<AutofacOptionsTests>();

            IServiceCollection services = new ServiceCollection() as IServiceCollection;
            services.AddLogging();

            IServiceProvider serviceProvider = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .IdentifyTenantTask(() => Task.FromResult(new TenantIdentifier(new Uri("unittest://"))))
                    .AddAspNetCore()
                    .InitialiseTenant(tenantId =>
                    {                        
                        if (tenantId.Uri.Scheme != "unittest")
                        {
                            throw new ArgumentException();                            
                        }

                        return new TenantShell<Tenant>(new Tenant());
                    })

                    .ConfigureTenantContainers((containerBuilder) =>
                    {
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
                        .WithAutofac((tenant, tenantServices) =>
                        {
                            tenantServices.AddOptions();
                            tenantServices.Configure<MyOptions>((a) => { a.Foo = true; });
                        })
                        .AddPerRequestContainerMiddlewareServices();
                    });
            });

            IServiceScopeFactory scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            IServiceScope scoped = scopeFactory.CreateScope();


            ITenantRequestContainerAccessor<Tenant> tenantRequestContainerAccessor = scoped.ServiceProvider.GetRequiredService<ITenantRequestContainerAccessor<Tenant>>();
            PerRequestContainer requestContainer = await tenantRequestContainerAccessor.TenantRequestContainer.Value;

            IOptions<MyOptions> registeredoptions = requestContainer.RequestContainer.GetRequiredService<IOptions<MyOptions>>();
            var myOptions = registeredoptions.Value;

            Assert.True(myOptions.Foo);
        }
    }
}




