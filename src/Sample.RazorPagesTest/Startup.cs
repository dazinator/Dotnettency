using Dotnettency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

            services.AddMvc();

            IServiceProvider serviceProvider = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .InitialiseTenant<TenantShellFactory>() // factory class to load tenant when it needs to be initialised for the first time. Can use overload to provide a delegate instead.                    
                    .AddAspNetCore()
                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                        containerBuilder.Events((events) =>
                        {
                            //// callback invoked after tenant container is created.
                            //events.OnTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
                            //{
                            //    var tenant = await tenantResolver;

                            //})
                            //// callback invoked after a nested container is created for a tenant. i.e typically during a request.
                            //.OnNestedTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
                            //{
                            //    var tenant = await tenantResolver;

                            //});
                        })
                        // Extension methods available here for supported containers. We are using structuremap..
                        // We are using an overload that allows us to configure structuremap with familiar IServiceCollection.
                        .WithAutofac((tenant, tenantServices) =>
                        {
                            //  var actionContextAccessor = new MyActionContextAccessor();
                            // tenantServices.AddSingleton<IActionContextAccessor>(actionContextAccessor);

                            // var mvcBuilder = tenantServices.AddMvc();

                            tenantServices.Configure<CookiePolicyOptions>(o =>
                            {
                                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                                o.CheckConsentNeeded = context => true;
                                o.MinimumSameSitePolicy = SameSiteMode.None;
                            });


                            //  tenantServices.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

                            //   mvcBuilder.AddRazorPagesOptions((r) => { r. });

                        })
                        .AddPerRequestContainerMiddlewareServices()
                        .AddPerTenantMiddlewarePipelineServices(); // allows tenants to have there own middleware pipeline accessor stored in their tenant containers.
                                                                   // .WithModuleContainers(); // Creates a child container per IModule.
                    })
                    .ConfigureTenantMiddleware((a) =>
                    {
                        a.OnInitialiseTenantPipeline((b, c) =>
                        {
                            c.UseDeveloperExceptionPage();
                            c.UseStaticFiles();

                            //  var log = c.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                            if (b.Tenant.Name == "Moogle")
                            {
                                c.UseCookiePolicy();
                            }
                            //if (env.IsDevelopment())
                            //{
                           
                            //}
                            //else
                            //{
                            // c.UseExceptionHandler("/Error");
                            //}

                          
                          
                            c.UseMvc();

                            //c.UseWelcomePage("/welcome");

                            //c.UseStaticFiles();
                            //UseMvc(c);

                        });
                    });

            });

            // When using tenant containers, must return IServiceProvider.
            return serviceProvider;


            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});


            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app = app.UseMultitenancy<Tenant>((options) =>
            {
                options.UsePerTenantContainers();
                options.UsePerTenantMiddlewarePipeline();
            });


            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //}

            //app.UseStaticFiles();
            //app.UseCookiePolicy();

            //app.UseMvc();
        }
    }
}
