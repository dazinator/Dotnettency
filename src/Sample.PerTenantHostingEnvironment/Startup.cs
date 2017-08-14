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

namespace Sample.PerTenantHostingEnvironment
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
            var serviceProvider = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .DistinguishTenantsBySchemeHostnameAndPort() // The distinguisher used to identify one tenant from another.
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
                        containerBuilder.WithStructureMapServiceCollection((tenant, tenantServices) =>
                        {
                            tenantServices.AddSingleton<SomeTenantService>();
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

            //  app.UseMiddleware<SampleMiddleware<Tenant>>();
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


                var messageBuilder = new StringBuilder();

                string tenantShellId = tenantShell == null ? "{NULL TENANT SHELL}" : tenantShell.Id.ToString();
                string tenantName = tenant == null ? "{NULL TENANT}" : tenant.Name;
                string injectedTenantName = someTenantService?.TenantName == null ? "{NULL TENANT}" : someTenantService?.TenantName;

                string fileContent = someTenantService?.GetContentFile("/Info.txt");
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                var result = new
                {
                    TenantShellId = tenantShellId,
                    TenantName = tenantName,
                    TenantScopedServiceId = someTenantService?.Id,
                    InjectedTenantName = injectedTenantName,
                    TenantContentFile = fileContent
                };

                var jsonResult = JsonConvert.SerializeObject(result);
                await context.Response.WriteAsync(jsonResult, Encoding.UTF8);


                //    context.Response.

                // for null tenants we could optionally redirect somewhere?
            });
        }
    }
}
