using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Dotnettency;
using System;

namespace Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .DistinguishTenantsByHostname()
                    .OnResolveTenant((distinguisher) => // invoked when tenant needs to be resolved, result is cached.
                     {
                         // The distinguisher used for this request - we are using hostname - return tenant accordingly.
                         if (distinguisher.Key == "localhost")
                         {
                             var tenant = new Tenant() { Name = "Test", Id = 1 };
                             var result = new TenantShell<Tenant>(tenant);
                             return result;
                         }

                         if (distinguisher.Key == "foo.com")
                         {
                             var tenant = new Tenant() { Name = "Foo and Bar and Baz", Id = 2 };
                             var result = new TenantShell<Tenant>(tenant, "bar.com", "baz.com");  // optional other identifies we want to associate this same tenant shell with.                          
                             return result;
                         }
                         // returning null will cause this method to keep being called on future requests with same distinguisher.                  
                         // if you would like to prevent this, return a new TenantShell<Tenant>(null) which will cause a null tenant to be cached.                 

                         return null;
                     })
                    .ConfigureTenantMiddleware((middlewareOptions) =>
                    {
                        middlewareOptions.OnBuildPipeline((context, appBuilder) =>
                        {
                            if (context.Tenant.Id == 1)
                            {
                                appBuilder.UseWelcomePage("/welcome");
                            }
                        });
                    })
                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                        containerBuilder.ConfigureStructureMapContainer((tenant, configuration) =>
                        {
                            configuration.ForSingletonOf<SomeTenantService>();
                            // could register services based on the tenant.
                            //switch (tenant.Id)
                            //{
                            //    case 1:                                   

                            //        break;
                            //    case 2:

                            //        break;
                            //}

                        });
                    });
            });

            // When using tenant containers, must return IServiceProvider.
            return serviceProvider;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add the multitenancy middleware.
            app.UseMultitenancy<Tenant>((options) =>
            {
                options.UsePerTenantMiddlewarePipeline()
                       .UsePerTenantContainers();
            });


            //  app.
            app.Run(async (context) =>
            {
                // Use ITenantAccessor to access the current tenant.
                var tenantAccessor = context.RequestServices.GetRequiredService<ITenantAccessor<Tenant>>();
                var tenant = await tenantAccessor.CurrentTenant.Value;

                // This service was registered as singleton in tenant container.
                var someTenantService = context.RequestServices.GetService<SomeTenantService>();
                await context.Response.WriteAsync("Hello tenant: " + tenant?.Name + ", Service Id: " + someTenantService?.Id);
            });
        }
    }
}
