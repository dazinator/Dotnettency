using Owin;
using System;
using Dotnettency;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Hosting;

namespace Sample.Owin.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {

            // Tenants all have different urls.
            var startOptions = new StartOptions("http://*:5000");
            startOptions.Urls.Add("http://*:5001");
            startOptions.Urls.Add("http://*:5002");
            startOptions.Urls.Add("http://*:5003");
            startOptions.Urls.Add("http://*:5004");


            using (WebApp.Start<Startup>(startOptions))
            {
                Console.WriteLine("Press [enter] to quit...");
                Console.ReadLine();
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            IServiceCollection services = new ServiceCollection();
            Configure(services);
            IServiceProvider sp = services.BuildServiceProvider();


#if DEBUG

            app.UseErrorPage();

            app.UseMultitenancy<Tenant>(sp, (options) =>
            {
               
                app.Use(async (context, next) =>
                {
                    // Todo: could offer an Extension method on IOwinContext for this.
                    var requestScopeAccessor = sp.GetRequiredService<ICurrentRequestItemAccessor<IServiceScope>>();
                    var requestServices = requestScopeAccessor.GetCurrent().ServiceProvider;

                    await context.Response.WriteAsync($"Browse on ports 5000 - 5004 to witness multitenancy behaviours.");
                   
                    var tenantResolver = requestServices.GetRequiredService<Task<Tenant>>();
                    var tenant = await tenantResolver;

                    if (tenant == null)
                    {
                        await context.Response.WriteAsync($"No Tenant mapped to this url!");
                    }
                    else
                    {
                        await context.Response.WriteAsync($"Hello from tenant: {tenant.Name}");
                    }

                    await next();

                });


                //  Optionally use tenant containers before starting request scope();
                //options.UseRequestScope(() => sp.CreateScope());
            });
            // above method needs to automatically do following:
            // 1. app.UseRequestScopeContext()
            // 2. Add middleware that will take the root sp, and create a request scope() sp, before onward processing of the request
            //     - Dotnettency should provide a way to expose the newly created request scope sp - so that:
            //           - can directly resolve the current tenant using it.
            //           - can set web api's / framework x's DependencyResolver.Current to use same one.





         


           

            // There is no middleware required for Tenant resolution itself, as it is a case of 
            // resolving the Tenant from the configured sp.
            // i.e could inject it into middleware.

            // The following middleware options are for advanced cases, such as:

            // 1. Per Tenant Container (Allows you to configure a ServiceProvider per tenant. 
            //    You can still configure services at a global IServiceProvider level as normal.
            //    Services configured at tenant level will override global level services.
            //    You have to use the dotnettency.container.autofac package to configure these containers.
            //    Then use the container middlewaruse an autofac backed IServiceProvider implementation,

            //app.UseMultitenancy<Tenant>(sp, (a) =>
            //{




            //});




#endif
            //  app.UseWelcomePage("/");
        }

        private void Configure(IServiceCollection services)
        {
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.AddOwin()
                       .IdentifyTenantsWithRequestAuthorityUri()
                       .InitialiseTenant<TenantShellFactory>();

            });

            services.AddCurrentRequestItemAccessor<IServiceScope>((sp) =>
            {
                return sp.CreateScope();
            });
        }
    }


    // ...

    //public class DefaultDependencyResolver : IDependencyResolver
    //{
    //    private readonly IServiceProvider provider;

    //    public DefaultDependencyResolver(IServiceProvider provider)
    //    {
    //        this.provider = provider;
    //    }

    //    public object GetService(Type serviceType)
    //    {
    //        return provider.GetService(serviceType);
    //    }

    //    public IEnumerable<object> GetServices(Type serviceType)
    //    {
    //        return provider.GetServices(serviceType);
    //    }

    //    public IDependencyScope BeginScope()
    //    {
    //        return this;
    //    }

    //    public void Dispose()
    //    {
    //    }
    //}
}
