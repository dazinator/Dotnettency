using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Dotnettency;

namespace Sample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .DistinguishTenantsWithHostname()
                    .OnInitialiseTenant((distinguisher) => // invoked when tenant needs to be initialised.
                     {
                         // The distinguisher for the current request - we are using hostname.
                         if (distinguisher.Key == "localhost")
                         {
                             var tenant = new Tenant() { Name = "Test" };
                             var result = new TenantShell<Tenant>(tenant);
                             return result;
                         }

                         if (distinguisher.Key == "foo.com")
                         {
                             var tenant = new Tenant() { Name = "Foo and Bar and Baz" };
                             var result = new TenantShell<Tenant>(tenant, "bar.com", "baz.com");  // optional other identifies we want to associate this same tenant shell with.                          
                             return result;
                         }
                         // returning null will cause dotnettency to try to intialsie this tenant again on future requests for the same hostname.

                         // if you would like to prevent this, return a new TenantShell<Tenant>(null) which will initialise a null tenant.                   

                         return null;
                     })
                    .ConfigureTenantPipeline((pipelineOptions) =>
                    {
                        pipelineOptions.FromDelegate((context, appBuilder) =>
                        {
                            appBuilder.UseWelcomePage("/welcome");
                        });
                    });
                    
                    

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePerTenantMiddlewarePipeline<Tenant>();

            app.Run(async (context) =>
            {
                var tenantAccessor = context.RequestServices.GetRequiredService<ITenantAccessor<Tenant>>();
                var tenant = await tenantAccessor.CurrentTenant.Value;

                await context.Response.WriteAsync("Hello tenant: " + tenant?.Name);
            });
        }
    }
}
