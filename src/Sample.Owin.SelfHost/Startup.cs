using Owin;
using System;
using Dotnettency;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.Owin.SelfHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            IServiceCollection services = new ServiceCollection();
            Configure(services);
            IServiceProvider sp = services.BuildServiceProvider();

            app.UseErrorPage();

            app.UseMultitenancy<Tenant>(sp, (options) =>
            {
                // Creates IServiceScope.ServiceProvider for current request scoped services,
                // which you can then get access to from any middleware using IOwinContext.GetRequestServices() extension method.           
                options.UseRequestServices(() => sp.CreateScope()); // You could optionally also set your WebAPI or Framework of choices DependencyResolver.Current here to use same scope.
            });

            app.Use(async (context, next) =>
            {
                var tenant = await context.GetTenantAysnc<Tenant>();
                await context.Response.WriteAsync($"Browse on ports 5000 - 5004 to witness multitenancy behaviours.");
                if (tenant == null)
                {
                    await context.Response.WriteAsync($"No Tenant mapped to this url!");
                }
                else
                {
                    await context.Response.WriteAsync($"Hello from tenant: {tenant.Name}");
                }

                await next();

            });
        }

        private void Configure(IServiceCollection services)
        {
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.AddOwin()
                       .IdentifyTenantsWithRequestAuthorityUri()
                       .InitialiseTenant<TenantShellFactory>();

            });
        }
    }
}