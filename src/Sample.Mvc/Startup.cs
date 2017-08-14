using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Dotnettency;
using System;
using System.Text;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Sample.Mvc
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        public Startup(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            services.AddMvc();

            var serviceProvider = services.AddMultiTenancy<Tenant>((options) =>
            {
                options                   
                    .InitialiseTenant<TenantShellFactory>() // factory class to load tenant when it needs to be initialised for the first time. Can use overload to provide a delegate instead.                    
                    .ConfigureTenantMiddleware((middlewareOptions) =>
                    {
                        // This method is called when need to initialise the middleware pipeline for a tenant (i.e on first request for the tenant)
                        middlewareOptions.OnInitialiseTenantPipeline((context, appBuilder) =>
                        {
                            appBuilder.UseStaticFiles(); // This demonstrates static files middleware, but below I am also using per tenant hosting environment which means each tenant can see its own static files in addition to the main application level static files.

                            if (context.Tenant?.Name == "Foo")
                            {
                                appBuilder.UseWelcomePage("/welcome");
                            }
                        });
                    }) // Configure per tenant containers.
                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                        // Extension methods available here for supported containers. We are using structuremap..
                        // We are using an overload that allows us to configure structuremap with familiar IServiceCollection.
                        containerBuilder.WithStructureMap((tenant, tenantServices) =>
                        {
                            // tenantServices.AddSingleton<SomeTenantService>();
                        });
                    })
                // configure per tenant hosting environment.
                .ConfigurePerTenantHostingEnvironment(_environment, (tenantHostingEnvironmentOptions) =>
                {
                    tenantHostingEnvironmentOptions.OnInitialiseTenantContentRoot((contentRootOptions) =>
                    {
                        // WE use a tenant's guid id to partition one tenants files from another on disk.
                        // NOTE: We use an empty guid for NULL tenants, so that all NULL tenants share the same location.
                        var tenantGuid = (contentRootOptions.Tenant?.TenantGuid).GetValueOrDefault();
                        contentRootOptions.TenantPartitionId(tenantGuid)
                                           .AllowAccessTo(_environment.ContentRootFileProvider); // We allow the tenant content root file provider to access to the environments content root.
                    });

                    tenantHostingEnvironmentOptions.OnInitialiseTenantWebRoot((webRootOptions) =>
                    {
                        // WE use the tenant's guid id to partition one tenants files from another on disk.
                        var tenantGuid = (webRootOptions.Tenant?.TenantGuid).GetValueOrDefault();
                        webRootOptions.TenantPartitionId(tenantGuid)
                                           .AllowAccessTo(_environment.WebRootFileProvider); // We allow the tenant web root file provider to access the environments web root files.
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
                options
                       .UsePerTenantContainers()
                       .UsePerTenantHostingEnvironment((hostingEnvironmentOptions) =>
                       {
                           // using tenant content root and web root.
                           hostingEnvironmentOptions.UseTenantContentRootFileProvider();
                           hostingEnvironmentOptions.UseTenantWebRootFileProvider();
                       })
                       .UsePerTenantMiddlewarePipeline();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

         //   app.UseMvc();
        }
    }
}