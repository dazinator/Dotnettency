using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Dotnettency;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
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

        public bool UseDebugSources { get; set; } = false;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });

            var defaultServices = services.Clone();

            var sp1 = services.BuildServiceProvider();
            var partManager1 = sp1.GetService<ApplicationPartManager>();

            bool tenantMode = true;
            if (!tenantMode)
            {
                DebugServiceCollectionExtensions.AddRazorPagesDebug(services)
                  .AddNewtonsoftJson();

                var serviceProvider = services.BuildServiceProvider();
                var partManager = serviceProvider.GetService<ApplicationPartManager>();
            }


            services.AddMultiTenancy<Tenant>((builder) =>
             {
                 builder.IdentifyTenantsWithRequestAuthorityUri()
                        .InitialiseTenant<TenantShellFactory>()
                        .AddAspNetCore()
                        .ConfigureTenantContainers((containerOptions) =>
                        {
                            containerOptions
                            .SetDefaultServices(defaultServices)
                            .Autofac((tenant, tenantServices) =>
                            {
                                if (tenantMode)
                                {
                                    if (UseDebugSources)
                                    {
                                        tenantServices.AddRazorPagesDebug((o) =>
                                        {
                                            o.RootDirectory = $"/Pages/{tenant.Name}";
                                        }).AddNewtonsoftJson();
                                    }
                                    else
                                    {
                                        if (tenant != null)
                                        {
                                            tenantServices.AddRazorPages((o) =>
                                            {

                                                o.RootDirectory = $"/Pages/{tenant.Name}";
                                            }).AddNewtonsoftJson();
                                        }
                                    }
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

                                tenantAppBuilder.UseRouting();

                                if (context.Tenant != null)
                                {
                                    tenantAppBuilder.UseAuthorization();
                                    tenantAppBuilder.UseEndpoints(endpoints =>
                                    {
                                        if (UseDebugSources)
                                        {
                                            AddMappings(endpoints);
                                        }
                                        else
                                        {
                                            endpoints.MapRazorPages();
                                        }
                                    });
                                }

                            });
                        });

             });

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
        }
    }
}
