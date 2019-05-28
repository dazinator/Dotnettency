using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dotnettency;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Sample.AspNetCore30.RazorPages.Internal;

namespace Sample.Pages
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });

            ServiceCollectionExtensions.AddRazorPages(services)
                                //tenantServices
                                // .AddRazorPages()
                                .AddNewtonsoftJson();


            services.AddMultiTenancy<Tenant>((builder) =>
             {
                 builder.IdentifyTenantsWithRequestAuthorityUri()
                        .InitialiseTenant<TenantShellFactory>()
                        .AddAspNetCore()
                        .ConfigureTenantContainers((containerOptions) =>
                        {
                            containerOptions.Autofac((tenant, tenantServices) =>
                            {
                                //ServiceCollectionExtensions.AddRazorPages(tenantServices)
                                ////tenantServices
                                //// .AddRazorPages()
                                // .AddNewtonsoftJson();
                            });
                            //.AddPerTenantMiddlewarePipelineServices();
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
                                tenantAppBuilder.UseRouting();
                                tenantAppBuilder.UseAuthorization();
                                tenantAppBuilder.UseEndpoints(endpoints =>
                                {

                                    AddMappings(endpoints);
                                    //  endpoints.MapRazorPages();
                                });

                            });
                        });

             });

            //services.AddRazorPages()
            //   .AddNewtonsoftJson();


        }

        private PageActionEndpointDataSource AddMappings(IEndpointRouteBuilder endpoints)
        {

            var dataSource = endpoints.DataSources.OfType<PageActionEndpointDataSource>().FirstOrDefault();
            if (dataSource == null)
            {
                dataSource = endpoints.ServiceProvider.GetRequiredService<PageActionEndpointDataSource>();
                endpoints.DataSources.Add(dataSource);
            }

            return dataSource;

            ////var marker = endpoints.ServiceProvider.GetService<PageActionEndpointDataSource>();
            ////if (marker == null)
            ////{
            ////    throw new InvalidOperationException(Mvc.Core.Resources.FormatUnableToFindServices(
            ////        nameof(IServiceCollection),
            ////        "AddRazorPages",
            ////        "ConfigureServices(...)"));
            ////}

            //// Arrange
            //var actions = new List<ActionDescriptor>
            //{
            //    new PageActionDescriptor
            //    {
            //        AttributeRouteInfo = new AttributeRouteInfo()
            //        {
            //            Template = "/index",
            //        },
            //        RouteValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            //        {
            //            { "action", "Index" },
            //            { "controller", "Test" },
            //        },
            //    },
            //};

            //var mockDescriptorProvider = new Mock<IActionDescriptorCollectionProvider>();
            //mockDescriptorProvider.Setup(m => m.ActionDescriptors).Returns(new ActionDescriptorCollection(actions, 0));

            //var dataSource = (PageActionEndpointDataSource)CreateDataSource(mockDescriptorProvider.Object);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMultitenancy<Tenant>((builder) =>
            {
                builder.UseTenantContainers()
                       .UsePerTenantMiddlewarePipeline(app);
            });

            //app.UseRouting();
            //app.UseAuthorization();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //});
        }
    }
}
