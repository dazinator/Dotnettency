using Owin;
using System;
using Dotnettency;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Owin.SelfHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            IServiceCollection services = new ServiceCollection();
            var sp = Configure(services);

            app.UseErrorPage();

            app.UseMultitenancy<Tenant>(sp, (options) =>
            {
                // Have to establish a scoped IServiceProvider for the current request first,
                // because there are dotnettency services that provide tenant level stuff, 
                // which are registered as Scoped() and so require this. Without this, they will be created in Application scope and stuff won't work!
                options.UseRequestServices(() => sp.CreateScope()); // You could optionally also set your WebAPI or Framework of choices DependencyResolver.Current here to use same scope.

                // Note: You can then get access to the current RequestServices from anywhere during a request, 
                // using IOwinContext.GetRequestServices() extension method.              
                options.UseTenantContainers(); // Will resolve current tenant's container, then swap out RequestServices for a tenant scoped IServiceProvider.

                options.UsePerTenantMiddlewarePipeline(app);

            });


            app.Use(async (context, next) =>
            {
                // Tenant resolution
                var tenant = await context.GetTenantAysnc<Tenant>();
                await context.Response.WriteAsync($"Browse on ports 5000 - 5004 to witness multitenancy behaviours. Also /Welcome for tenant 'Bar' has welcome page middleware but other tenants don't!");
                if (tenant == null)
                {
                    await context.Response.WriteAsync($"No Tenant mapped to this url!");
                }
                else
                {
                    await context.Response.WriteAsync($"Hello from tenant: {tenant.Name}");
                }

                // Tenant services (containers).
                var requestSp = context.GetRequestServices();
                var service = requestSp.GetService<SomeTenantService>();
               
                if (service == null)
                {
                    await context.Response.WriteAsync($"SomeTenantService is null");
                }
                else
                {
                    await context.Response.WriteAsync($"SomeTenantService is: Name: {service.TenantName}, Id: {service.Id}");
                }

                await next();

            });


        }

        private IServiceProvider Configure(IServiceCollection services)
        {
            services.AddLogging();

            var sp = services.AddMultiTenancy<Tenant>((builder) =>
             {
                 builder.AddOwin()
                         .Map<int>((m) =>
                         {
                             m.SelectRequestHost()
                              .WithMapping((tenants) =>
                              {
                                  tenants.Add(1, "t1.foo.com", "t1.foo.uk");
                              })
                              .UsingDotNetGlobPatternMatching();
                         })
                        .ConfigureTenantContainers((containerOptions) =>
                        {
                            containerOptions.Autofac((tenantContext, tenantServices) =>
                            {
                                tenantServices.AddSingleton<SomeTenantService>(new SomeTenantService(tenantContext.Tenant));
                            });
                        })
                        .ConfigureTenantMiddleware((b) =>
                        {
                            b.OwinPipeline((p, c) =>
                            {
                                var tenant = p.Tenant;
                                if (tenant?.Name == "Bar")
                                {
                                    c.UseWelcomePage(new Microsoft.Owin.Diagnostics.WelcomePageOptions()
                                    {
                                        Path = new Microsoft.Owin.PathString("/Welcome")
                                    });
                                }
                            });
                        });
                        
             });

            // Note: You must return / use dotnettency's IServiceProvider, as it is backed by autofac in this instance, and has additional services that won't be contained in your original IServiceCollecton.
            return sp;
        }
    }
}