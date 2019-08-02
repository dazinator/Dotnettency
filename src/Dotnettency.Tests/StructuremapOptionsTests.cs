using Dotnettency.Container;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StructureMap;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Dotnettency.Tests
{

    public class MyOptions
    {
        public bool Prop { get; set; }
        public bool Foo { get; internal set; }
    }
    public class StructuremapOptionsTests
    {


        [Fact]
        public void Options_Configure_Root_Resolve_Root()
        {           
            // ServiceProvider serviceProvider = services.BuildServiceProvider();
            StructureMap.Container container = new StructureMap.Container();
            container.Configure((a)=> {

                ServiceCollection services = new ServiceCollection();
                services.AddOptions();
                services.Configure<MyOptions>((b) =>
                {
                    b.Prop = true;
                });
                a.Populate(services);
            });

            // container.Populate(services);

            IServiceProvider sp = container.GetInstance<IServiceProvider>();
            IOptions<MyOptions> options = sp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Prop);

        }


        [Fact]
        public void Options_Populate_Root_Resolve_Root()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.Configure<MyOptions>((a) =>
            {
                a.Prop = true;
            });
           // ServiceProvider serviceProvider = services.BuildServiceProvider();


            StructureMap.Container container = new StructureMap.Container();
            container.Populate(services);

            // container.Populate(services);

            IServiceProvider sp = container.GetInstance<IServiceProvider>();
            IOptions<MyOptions> options = sp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Prop);

        }

        //[Fact]
        //public void Options_Configure_Child_Resolve_Child()
        //{

        //    ServiceCollection services = new ServiceCollection();         
          
        //    StructureMap.Container container = new StructureMap.Container();
        //    container.Populate(services);

        //    var childContainer = container.CreateChildContainer();
        //    childContainer.Configure((a) =>
        //    {
        //        var childServices = new ServiceCollection();
        //        childServices.AddOptions();
        //        childServices.Configure<MyOptions>((b) =>
        //        {
        //            b.Prop = true;
        //        });
        //        a.Populate(childServices);
        //    });

        //    // container.Populate(services);

        //    IServiceProvider sp = childContainer.GetInstance<IServiceProvider>();
        //    IOptions<MyOptions> options = sp.GetRequiredService<IOptions<MyOptions>>();
        //    Assert.True(options.Value?.Prop);

        //}

        //[Fact]
        //public void Options_Populate_Child_Resolve_Child()
        //{

        //    ServiceCollection services = new ServiceCollection();

        //    StructureMap.Container container = new StructureMap.Container();
        //    container.Populate(services);

        //    var childContainer = container.CreateChildContainer();

        //    var childServices = new ServiceCollection();
        //    childServices.AddOptions();
        //    childServices.Configure<MyOptions>((b) =>
        //    {
        //        b.Prop = true;
        //    });

        //    childContainer.Populate(childServices);            

        //    // container.Populate(services);

        //    IServiceProvider sp = childContainer.GetInstance<IServiceProvider>();
        //    IOptions<MyOptions> options = sp.GetRequiredService<IOptions<MyOptions>>();
        //    Assert.True(options.Value?.Prop);

        //}

        [Fact]
        public void Options_Populate_Root_Resolve_Root_Using_TenantContainerAdaptor()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.Configure<MyOptions>((a) =>
            {
                a.Prop = true;
            });
            ServiceProvider serviceProvider = services.BuildServiceProvider();


            StructureMap.Container container = new StructureMap.Container();
            Dotnettency.Container.StructureMap.ContainerExtensions.Populate(container, services);

            // container.Populate(services);

            ITenantContainerAdaptor sp = container.GetInstance<ITenantContainerAdaptor>();
            IOptions<MyOptions> options = sp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Prop);

        }

        [Fact]
        public void Options_Populate_Root_Resolve_Child_Using_TenantContainerAdaptor()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.Configure<MyOptions>((a) =>
            {
                a.Prop = true;
            });
            ServiceProvider serviceProvider = services.BuildServiceProvider();


            StructureMap.Container container = new StructureMap.Container();
            Dotnettency.Container.StructureMap.ContainerExtensions.Populate(container, services);

            // container.Populate(services);

            ITenantContainerAdaptor sp = container.GetInstance<ITenantContainerAdaptor>();

            ITenantContainerAdaptor childSp = sp.CreateChildContainer("Child");

            IOptions<MyOptions> options = childSp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Prop);


        }

        [Fact]
        public void Options_Populate_Root_Resolve_Nested_Using_TenantContainerAdaptor()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.Configure<MyOptions>((a) =>
            {
                a.Prop = true;
            });
            ServiceProvider serviceProvider = services.BuildServiceProvider();


            StructureMap.Container container = new StructureMap.Container();
            Dotnettency.Container.StructureMap.ContainerExtensions.Populate(container, services);

            // container.Populate(services);

            ITenantContainerAdaptor sp = container.GetInstance<ITenantContainerAdaptor>();

            ITenantContainerAdaptor childSp = sp.CreateChildContainer("Child");
            var nestedSp = childSp.CreateChildContainer("Nested");


            IOptions<MyOptions> options = nestedSp.GetRequiredService<IOptions<MyOptions>>();
            Assert.True(options.Value?.Prop);

        }

        [Fact]
        public async Task Options_Configure_Child_Resolve_Child_Using_TenantContainerAdaptor()
        {

            ServiceCollection services = new ServiceCollection();
            services.AddLogging();

          //  ServiceProvider serviceProvider = services.BuildServiceProvider();


            StructureMap.Container container = new StructureMap.Container();
            Dotnettency.Container.StructureMap.ContainerExtensions.Populate(container, services);

            var adaptedContainer = container.GetInstance<ITenantContainerAdaptor>();
            var containerEventsPublisher = container.TryGetInstance<ITenantContainerEventsPublisher<MyTenant>>();

            container.Configure(_ =>
                       _.For<ITenantContainerBuilder<MyTenant>>()
                           .Use(new DelegateActionTenantContainerBuilder<MyTenant>(services, adaptedContainer, (s, t)=> {

                               t.AddOptions();
                               t.Configure<MyOptions>((a) =>
                               {
                                   a.Prop = true;
                               });

                           }, containerEventsPublisher))
                       );

            // container.Populate(services);

            var tenantContainerBuilder = adaptedContainer.GetRequiredService<ITenantContainerBuilder<MyTenant>>();
            var tenantContainer = await tenantContainerBuilder.BuildAsync(new MyTenant());

            IOptions<MyOptions> options = tenantContainer.GetRequiredService<IOptions<MyOptions>>();        
            Assert.True(options.Value?.Prop);

        }

     
    }

    public class MyTenant
    {

    }



}




