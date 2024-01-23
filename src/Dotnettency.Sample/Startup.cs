using Dotnettency;
using Dotnettency.AspNetCore.Modules;
using Dotnettency.Container;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{

    public class MyOptions
    {

        public bool Foo { get; set; }


    }


    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;

        public Startup(IWebHostEnvironment environment, ILoggerFactory loggerFactory)
        {
            _environment = environment;
            _loggerFactory = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
          //  var platformServices = services.Clone();
            services.AddRouting();
            
           //_loggerFactory.AddConsole();
            ILogger<Startup> logger = _loggerFactory.CreateLogger<Startup>();

            services = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .AddAspNetCore()
                    .IdentifyTenantsWithRequestAuthorityUri()
                    .InitialiseTenant<TenantShellFactory>() // factory class to load tenant when it needs to be initialised for the first time. Can use overload to provide a delegate instead.                    
                    .ConfigureTenantContainers((containerBuilder) =>
                    {
                       // containerBuilder.SetDefaultServices(platformServices);
                        containerBuilder.Events((events) =>
                            {
                                // callback invoked after tenant container is created.
                                events.OnTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
                                    {
                                        Tenant tenant = await tenantResolver;

                                    })
                                    // callback invoked after a nested container is created for a tenant. i.e typically during a request.
                                    .OnNestedTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
                                    {
                                        Tenant tenant = await tenantResolver;

                                    });
                            })
                            // Extension methods available here for supported containers. We are using structuremap..
                            // We are using an overload that allows us to configure structuremap with familiar IServiceCollection.
                            .Native((tenantContext, tenantServices) =>
                            {
                                tenantServices.AddOptions();
                                tenantServices.Configure<MyOptions>((a) => { a.Foo = true; });

                                tenantServices.AddSingleton<SomeTenantService>((sp) =>
                                {
                                    //var logger = sp.GetRequiredService<ILogger<SomeTenantService>>();
                                    logger.LogDebug("Resolving SomeTenantService");
                                    var hostingEnv = sp.GetRequiredService<IWebHostEnvironment>();
                                    return new SomeTenantService(tenantContext.Tenant, hostingEnv);
                                });

                                tenantServices.AddModules<ModuleBase>((modules) =>
                                {
                                    // Only load these two modules for tenant Bar.
                                    if (tenantContext.Tenant?.Name == "Bar")
                                    {
                                        modules.AddModule<SampleRoutedModule>()
                                            .AddModule<SampleSharedModule>();
                                    }

                                    modules.ConfigureModules();


                                });
                            });
                        // .WithModuleContainers(); // Creates a child container per IModule.
                    })
                    .ConfigureTenantMiddleware((tenantBuilder) =>
                    {
                        tenantBuilder.AspNetCorePipeline((tenantContext, tenantAppBuilder) =>
                        {
                            var startupLogger = tenantAppBuilder.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                            startupLogger.LogDebug("Configuring tenant middleware pipeline for tenant: " + tenantContext.Tenant?.Name ?? "");
                            // appBuilder.UseStaticFiles(); // This demonstrates static files middleware, but below I am also using per tenant hosting environment which means each tenant can see its own static files in addition to the main application level static files.

                            tenantAppBuilder.UseModules<Tenant, ModuleBase>();

                            // welcome page only enabled for tenant FOO.
                            if (tenantContext.Tenant?.Name == "Foo")
                            {
                                tenantAppBuilder.UseWelcomePage("/welcome");
                            }

                            // display info.
                            tenantAppBuilder.Run(DisplayInfo);
                        });

                    })

                    // configure per tenant hosting environment.
                    .ConfigureNamedTenantFileSystems((namedItems) =>
                    {
                        var contentFileProvider = _environment.ContentRootFileProvider;
                        var webFileProvider = _environment.WebRootFileProvider;

                        namedItems.AddWebFileSystem(_environment.WebRootPath, (ctx, fs) =>
                            {
                                Guid tenantGuid = (ctx.Tenant?.TenantGuid).GetValueOrDefault();
                                fs.SetPartitionId(tenantGuid)
                                    .SetSubDirectory(".tenants\\")
                                    .AllowAccessTo(webFileProvider);
                            })
                            .UseAsEnvironmentWebRootFileProvider(_environment)
                            .AddContentFileSystem(_environment.ContentRootPath, (ctx, fs) =>
                            {
                                Guid tenantGuid = (ctx.Tenant?.TenantGuid).GetValueOrDefault();
                                fs.SetPartitionId(tenantGuid)
                                    .SetSubDirectory(".tenants\\")
                                    .AllowAccessTo(contentFileProvider);
                            })
                            .UseAsEnvironmentContentRootFileProvider(_environment);
                    });

            });

            //services.AddChildContainers(); // required if not using IHost IServiceProviderFactory

            // When using tenant containers, must return IServiceProvider.
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //  loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app = app.UseMultitenancy<Tenant>((options) =>
            {
                options.UseTenantContainers();               
                options.UsePerTenantMiddlewarePipeline(app);

            });


            //app.UseRouter(((routeBuilder) =>
            //{
            //    // Makes sure that should any child route match, then the tenant container is restored prior to that route handling the request.
            //    routeBuilder.EnsureTenantContainer<Tenant>((childRouteBuilder) =>
            //    {
            //        // Adds a route that will handle the request via the current tenants middleware pipleine. 
            //        childRouteBuilder.MapTenantMiddlewarePipeline<Tenant>((context, appBuilder) =>
            //        {

            //            ILogger<Startup> logger = appBuilder.ApplicationServices.GetRequiredService<ILogger<Startup>>();
            //            logger.LogDebug("Configuring tenant middleware pipeline for tenant: " + context.Tenant?.Name ?? "");
            //            // appBuilder.UseStaticFiles(); // This demonstrates static files middleware, but below I am also using per tenant hosting environment which means each tenant can see its own static files in addition to the main application level static files.

            //            appBuilder.UseModules<Tenant, ModuleBase>();

            //            // welcome page only enabled for tenant FOO.
            //            if (context.Tenant?.Name == "Foo")
            //            {
            //                appBuilder.UseWelcomePage("/welcome");
            //            }
            //            // display info.
            //            appBuilder.Run(DisplayInfo);

            //        }); // handled by the tenant's middleware pipeline - if there is one.                  
            //    });
            //}));

            // This will only be reached if no routes were resolved above.
            // app.Run(DisplayInfo);



        }

        public async Task DisplayInfo(HttpContext context)
        {
            ILogger<Startup> logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
            logger.LogDebug("App Run..");

            ITenantContainerAdaptor container = context.RequestServices as ITenantContainerAdaptor;
            logger.LogDebug("App Run Container Is: {id}, {containerNAme}, {role}", container.ContainerId, container.ContainerName, container.Role);


            // Use ITenantAccessor to access the current tenant.
            ITenantAccessor<Tenant> tenantAccessor = container.GetRequiredService<ITenantAccessor<Tenant>>();
            Tenant tenant = await tenantAccessor.CurrentTenant.Value;

            // This service was registered as singleton in tenant container.
            SomeTenantService someTenantService = container.GetService<SomeTenantService>();

            // The tenant shell to access context for the tenant - even if the tenant is null
            ITenantShellAccessor<Tenant> tenantShellAccessor = context.RequestServices.GetRequiredService<ITenantShellAccessor<Tenant>>();
            TenantShell<Tenant> tenantShell = await tenantShellAccessor.CurrentTenantShell.Value;

            var myOptions = context.RequestServices.GetRequiredService<IOptions<MyOptions>>();

            string tenantShellId = tenantShell == null ? "{NULL TENANT SHELL}" : tenantShell.Id.ToString();
            string tenantName = tenant == null ? "{NULL TENANT}" : tenant.Name;
            string injectedTenantName = someTenantService?.TenantName ?? "{NULL SERVICE}";

            // Accessing a content file.
            string fileContent = someTenantService?.GetContentFile("/Info.txt");
            context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
            var result = new
            {
                TenantShellId = tenantShellId,
                TenantName = tenantName,
                TenantScopedServiceId = someTenantService?.Id,
                InjectedTenantName = injectedTenantName,
                TenantContentFile = fileContent,
                OptionsFoo = myOptions.Value.Foo
            };

            string jsonResult = JsonConvert.SerializeObject(result);
            await context.Response.WriteAsync(jsonResult, Encoding.UTF8);
            logger.LogDebug("App Run Finished..");
        }


    }
}
