using Dotnettency;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Sample.TenantAuthentication
{
    public partial class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            bool challenged = false;

         //   services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
         //.AddCookie(options => {
         //    options.LoginPath = "/Account/Login/";
         //});

            return services.AddAspNetCoreMultiTenancy<Tenant>((multiTenancyOptions) =>
            {
                multiTenancyOptions
                    .InitialiseTenant<TenantShellFactory>()
                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                        containerBuilder.WithStructureMap((tenant, tenantServices) =>
                        {
                            if (tenant.Name == "Moogle")
                            {

                                tenantServices.AddSingleton<IPostConfigureOptions<BasicAuthenticationOptions>, BasicAuthenticationPostConfigureOptions>();
                                tenantServices.AddSingleton<IBasicAuthenticationService, BasicAuthenticationService>();
                                //  tenantServices.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                                tenantServices.AddAuthenticationCore().AddAuthentication("Basic")
                                    .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);

                            }
                            else
                            {

                                tenantServices.AddSingleton<IPostConfigureOptions<BasicAuthenticationOptions>, BasicAuthenticationPostConfigureOptions>();
                                tenantServices.AddSingleton<IBasicAuthenticationService, BasicAuthenticationService>();
                                //  tenantServices.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                                tenantServices.AddAuthentication("Basic")
                                    .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>("Basic", null);


                            }
                        }).AddPerRequestContainerMiddlewareServices();
                    }).ConfigureTenantMiddleware((b) => b.OnInitialiseTenantPipeline((tenant, app) =>
                    {
                        app.UseMiddleware<Sample.TenantAuthentication.Authentication.AuthenticationMiddleware>();

                        
                        app.Run(async (context) =>
                        {
                            if(!challenged)
                            {
                                challenged = true;
                                await context.ChallengeAsync("Basic");
                            }
                            await context.Response.WriteAsync("Hello World!");
                        });
                    }));
            });



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMultitenancy<Tenant>((options) =>
            {
                options.UsePerTenantContainers()
                       .UsePerTenantMiddlewarePipeline();
            });



        }
    }
}
