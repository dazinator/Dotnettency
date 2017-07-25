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
                    .DistinguishTenantsBySchemeHostnameAndPort()  // The distinguisher used to identify one tenant from another. Other methods available.
                    .OnResolveTenant((distinguisher) => // method used to resolve the tenant using the distinguisher above.
                     {
                         if (distinguisher.Key == "http://localhost:63291")
                         {
                             var tenant = new Tenant() { Name = "Foo", Id = 1 };
                             var result = new TenantShell<Tenant>(tenant);
                             return result;
                         }

                         if (distinguisher.Key.Contains(":5000") || distinguisher.Key.Contains(":5001"))
                         {
                             var tenant = new Tenant() { Name = "Bar", Id = 2 };
                             var result = new TenantShell<Tenant>(tenant, "http://localhost:5000", "http://localhost:5001"); // additional distinguishers to map this same tenant shell instance too.
                             return result;
                         }

                         // for an unknown tenant, we can either create the tenant shell as a NULL tenant by returning a TenantShell<TTenant>(null),
                         // which results in the TenantShell being created, and will explicitly have to be reloaded() in order for this method to be called again.                        
                         if (distinguisher.Key.Contains("5002"))
                         {
                             var result = new TenantShell<Tenant>(null);
                             return result;
                         }

                         if (distinguisher.Key.Contains("5003"))
                         {

                             // or we can return null - which means we wil keep attempting to resolve the tenant on every subsequent request until a result is returned in future.
                             // (i.e allows tenant to be created in backend in a few moments time). 
                             return null;
                         }

                         throw new NotImplementedException("Please make request on ports 5000 - 5003 to see various behaviour. Can also use 63291 when launching under IISExpress");


                     })
                    .ConfigureTenantMiddleware((middlewareOptions) =>
                    {
                        middlewareOptions.OnBuildPipeline((context, appBuilder) =>
                        {
                            if (context.Tenant?.Id == 1)
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

                // The tenant shell to access context for the tenant - even if the tenant is null
                var tenantShellAccessor = context.RequestServices.GetRequiredService<ITenantShellAccessor<Tenant>>();
                var tenantShell = await tenantShellAccessor.CurrentTenantShell.Value;

                string tenantShellId = tenantShell == null ? "{NULL TENANT SHELL}" : tenantShell.Id.ToString();
                string tenantName = tenant == null ? "{NULL TENANT}" : tenant.Name;

                var message = $"Tenant Shell Id: {tenantShellId} {Environment.NewLine} Tenant Name: {tenantName} {Environment.NewLine}Tenant Scoped Singleton Service Id: {someTenantService?.Id}";
                await context.Response.WriteAsync(message);

                // for null tenants we could optionally redirect somewhere?
            });
        }
    }
}
