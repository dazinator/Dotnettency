using Dotnettency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sample.RazorPages;
using System;

namespace Sample.RazorPagesTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
           // var defaultServices = services.Clone();

            var sp = services.AddMultiTenancy<Tenant>((builder) =>
              {
                  builder.IdentifyTenantsWithRequestAuthorityUri()
                         .InitialiseTenant<TenantShellFactory>()
                         .AddAspNetCore()
                         .ConfigureTenantContainers((containerOptions) =>
                         {
                             containerOptions
                            // .SetDefaultServices(defaultServices)
                             .Autofac((tenantContext, tenantServices) =>
                             {
                                 if (tenantContext.Tenant != null)
                                 {
                                     tenantServices.AddMvc((o) =>
                                     {

                                     }).AddRazorPagesOptions((o) =>
                                     {
                                         o.RootDirectory = $"/Pages/{tenantContext.Tenant.Name}";
                                     }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
                                 }
                             });
                         })
                         .ConfigureTenantMiddleware((tenantOptions) =>
                         {
                             tenantOptions.AspNetCorePipeline((context, tenantAppBuilder) =>
                             {

                                 tenantAppBuilder.Use(async (c, next) =>
                                 {
                                     Console.WriteLine("Entering tenant pipeline: " + context.Tenant?.Name);
                                     await next.Invoke();
                                 });

                                 if (context.Tenant != null)
                                 {
                                     tenantAppBuilder.UseMvc();
                                 }

                             });
                         });

              });

            return sp;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app = app.UseMultitenancy<Tenant>((options) =>
            {
                options.UseTenantContainers();
                options.UsePerTenantMiddlewarePipeline(app);
            });


            //app.UseStaticFiles();
            //app.UseCookiePolicy();

            //app.UseMvc();
        }
    }
}
