using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Dotnettency;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Dotnettency.Container;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace Sample.RazorPages
{

    public static class AttributeRouting
    {
        /// <summary>
        /// Creates an attribute route using the provided services and provided target router.
        /// </summary>
        /// <param name="services">The application services.</param>
        /// <returns>An attribute route.</returns>
        public static IRouter CreateAttributeMegaRoute(IServiceProvider services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var provider = services.GetRequiredService<IActionDescriptorCollectionProvider>();

            return new AttributeRoute(provider,
                services,
                actions =>
                {
                    var handler = services.GetRequiredService<MvcAttributeRouteHandler>();
                    handler.Actions = actions;
                    return handler;
                });
        }
    }

    public class MyActionContextAccessor : IActionContextAccessor
    {
        public ActionContext ActionContext { get; set; }
    }

    public class Startup
    {
        private readonly IHostingEnvironment _environment;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;

        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _environment = environment;
            _loggerFactory = loggerFactory;
            _configuration = configuration;
        }


        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // services.AddRouting();
           // services.AddMiddlewareAnalysis();
           // services.AddMvc();

            _loggerFactory.AddConsole();
            var logger = _loggerFactory.CreateLogger<Startup>();


            var serviceProvider = services.AddMultiTenancy<Tenant>((options) =>
            {
                options
                    .AddAspNetCore()
                    .InitialiseTenant<TenantShellFactory>() // factory class to load tenant when it needs to be initialised for the first time. Can use overload to provide a delegate instead.                    
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
                        .Autofac((tenant, tenantServices) =>
                        {
                          //  var actionContextAccessor = new MyActionContextAccessor();
                           // tenantServices.AddSingleton<IActionContextAccessor>(actionContextAccessor);

                           var mvcBuilder = tenantServices.AddMvc();
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
                            var log = c.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                            c.UseWelcomePage("/welcome");

                             c.UseStaticFiles();
                             UseMvc(c);

                        });
                    });

            });

            // When using tenant containers, must return IServiceProvider.
            return serviceProvider;
            // return services.BuildServiceProvider();
        }


        //// This method gets called by the runtime. Use this method to add services to the container.
        //// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        //public IServiceProvider ConfigureServices(IServiceCollection services)
        //{
        //    // services.AddRouting();
        //    services.AddMiddlewareAnalysis();

        //    _loggerFactory.AddConsole();
        //    var logger = _loggerFactory.CreateLogger<Startup>();



        //    var serviceProvider = services.AddAspNetCoreMultiTenancy<Tenant>((options) =>
        //    {
        //        options
        //            .InitialiseTenant<TenantShellFactory>() // factory class to load tenant when it needs to be initialised for the first time. Can use overload to provide a delegate instead.                    
        //            .ConfigureTenantContainers((containerBuilder) =>
        //            {
        //                containerBuilder.Events((events) =>
        //                {
        //                    // callback invoked after tenant container is created.
        //                    events.OnTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
        //                    {
        //                        var tenant = await tenantResolver;

        //                        //var routeHandler = tenantServiceProvider.GetRequiredService<MvcRouteHandler>();
        //                        //var routeContext = CreateRouteContext("Get", null);
        //                        //var routes = routeHandler.RouteAsync(CreateRouteContext());
        //                        // var diagnosticListener = tenantServiceProvider.GetRequiredService<DiagnosticListener>();
        //                        //  var listener = new TenantMiddlewareDiagnosticListener(tenant);
        //                        // diagnosticListener.SubscribeWithAdapter(listener);

        //                    })
        //                    // callback invoked after a nested container is created for a tenant. i.e typically during a request.
        //                    .OnNestedTenantContainerCreated(async (tenantResolver, tenantServiceProvider) =>
        //                    {
        //                        var tenant = await tenantResolver;

        //                    });
        //                })
        //                // Extension methods available here for supported containers. We are using structuremap..
        //                // We are using an overload that allows us to configure structuremap with familiar IServiceCollection.
        //                .WithStructureMap((tenant, tenantServices) =>
        //                {
        //                    //  ActionContextAccessor actionContextAccessor = null;
        //                    //  tenantServices.AddMvc();

        //                    tenantServices.AddMvc().AddRazorPagesOptions(razorOptions =>
        //                    {
        //                        razorOptions.RootDirectory = "/Pages";

        //                    });


        //                    //tenantServices.AddSingleton<MvcRouteHandler>((sp)=> {
        //                    //    var instance =  Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<MvcRouteHandler>(sp, actionContextAccessor);
        //                    //    return instance;
        //                    //});


        //                    //_.ForConcreteType<Thingie>().Configure

        //                    //// StructureMap parses the expression passed
        //                    //// into the method below to determine the
        //                    //// constructor
        //                    //.SelectConstructor(() => new Thingie(null));


        //                    // tenantServices.AddMiddlewareAnalysis();
        //                    var actionContextAccessor = new MyActionContextAccessor();
        //                    tenantServices.AddSingleton<IActionContextAccessor>(actionContextAccessor);
        //                    // tenantServices.AddMvc();

        //                })
        //                .AddPerRequestContainerMiddlewareServices()
        //                .AddPerTenantMiddlewarePipelineServices(); // allows tenants to have there own middleware pipeline accessor stored in their tenant containers.
        //                                                           // .WithModuleContainers(); // Creates a child container per IModule.
        //            })
        //            .ConfigureTenantMiddleware((a) =>
        //            {
        //                a.OnInitialiseTenantPipeline((b, c) =>
        //                {
        //                    var log = c.ApplicationServices.GetRequiredService<ILogger<Startup>>();

        //                    //  logger.LogDebug("Configuring tenant middleware pipeline for tenant: " + b.Tenant?.Name ?? "");                                                   
        //                    // c.UseWelcomePage("/welcome");
        //                    // var contextAccessor = c.ApplicationServices.GetRequiredService<IActionContextAccessor>();
        //                    c.UseWelcomePage();
        //                    UseMvcWithDefaultRoute(c);
        //                    c.Run(DisplayInfo);
        //                    // UseMvc(c);
        //                    // c.UseMvcWithDefaultRoute();
        //                    // c.UseMvc((r)=> { r.});
        //                    // display info.

        //                });
        //            });

        //    });

        //    // When using tenant containers, must return IServiceProvider.
        //    return serviceProvider;
        //    // return services.BuildServiceProvider();
        //}

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, DiagnosticListener diagnosticListener, IHostingEnvironment env)
        {

          //  var listener = new ApplicationMiddlewareDiagnosticListener();
          //  diagnosticListener.SubscribeWithAdapter(listener);

            //  app.UseMvc();

            app = app.UseMultitenancy<Tenant>((options) =>
          {
              options.UseTenantContainers();
              options.UsePerTenantMiddlewarePipeline();
          });

          // app.UseStaticFiles();
          // UseMvc(app);


            //  app.Run(DisplayInfo);

            //app.UseRouter(((routeBuilder) =>
            //{
            //    // Makes sure that should any child route match, then the tenant container is restored prior to that route handling the request.
            //    routeBuilder.EnsureTenantContainer<Tenant>((childRouteBuilder) =>
            //    {
            //        // Adds a route that will handle the request via the current tenants middleware pipleine. 
            //        childRouteBuilder.MapTenantMiddlewarePipeline<Tenant>((context, appBuilder) =>
            //        {

            //            var logger = appBuilder.ApplicationServices.GetRequiredService<ILogger<Startup>>();
            //            logger.LogDebug("Configuring tenant middleware pipeline for tenant: " + context.Tenant?.Name ?? "");


            //            if (env.IsDevelopment())
            //            {
            //                appBuilder.UseBrowserLink();
            //                appBuilder.UseDeveloperExceptionPage();
            //            }
            //            else
            //            {
            //                appBuilder.UseExceptionHandler("/Error");
            //            }

            //            appBuilder.UseStaticFiles();

            //            // appBuilder.UseStaticFiles(); // This demonstrates static files middleware, but below I am also using per tenant hosting environment which means each tenant can see its own static files in addition to the main application level static files.

            //            //  appBuilder.UseModules<Tenant, ModuleBase>();

            //            // welcome page only enabled for tenant FOO.
            //            if (context.Tenant?.Name == "Foo")
            //            {
            //                appBuilder.UseWelcomePage("/welcome");
            //            }

            //            appBuilder.UseMvc();
            //            // display info.
            //            // appBuilder.Run(DisplayInfo);

            //        }); // handled by the tenant's middleware pipeline - if there is one.                  
            //    });
            //}));


        }

        /// <summary>
        /// Adds MVC to the <see cref="IApplicationBuilder"/> request execution pipeline
        /// with a default route named 'default' and the following template:
        /// '{controller=Home}/{action=Index}/{id?}'.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseMvcWithDefaultRoute(IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return UseMvc(app, routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public static IApplicationBuilder UseMvc(IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return UseMvc(app, routes =>
            {
            });
        }


        /// <summary>
        /// Adds MVC to the <see cref="IApplicationBuilder"/> request execution pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="configureRoutes">A callback to configure MVC routes.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseMvc(
            IApplicationBuilder app,
            Action<IRouteBuilder> configureRoutes)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (configureRoutes == null)
            {
                throw new ArgumentNullException(nameof(configureRoutes));
            }

            // Verify if AddMvc was done before calling UseMvc
            // We use the MvcMarkerService to make sure if all the services were added.
            if (app.ApplicationServices.GetService(typeof(MvcMarkerService)) == null)
            {
                throw new InvalidOperationException();
            }

            var middlewarePipelineBuilder = app.ApplicationServices.GetRequiredService<MiddlewareFilterBuilder>();
            middlewarePipelineBuilder.ApplicationBuilder = app.New();
            middlewarePipelineBuilder.ApplicationBuilder.ApplicationServices = app.ApplicationServices;
            var handler = app.ApplicationServices.GetRequiredService<MvcRouteHandler>();

            var routes = new RouteBuilder(app)
            {
                DefaultHandler = handler,
            };

            configureRoutes(routes);

            routes.Routes.Insert(0, AttributeRouting.CreateAttributeMegaRoute(app.ApplicationServices));
            //       routes.MapRoute(
            //name: "foo",
            //template: "{controller=Home}/{action=Index}/{id?}");

            return app.UseRouter(routes.Build());
        }


        public async Task DisplayInfo(HttpContext context)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();
            logger.LogDebug("App Run..");

            var container = context.RequestServices as ITenantContainerAdaptor;
            logger.LogDebug("App Run Container Is: {id}, {containerNAme}, {role}", container.ContainerId, container.ContainerName, container.Role);


            // Use ITenantAccessor to access the current tenant.
            var tenantAccessor = container.GetRequiredService<ITenantAccessor<Tenant>>();
            var tenant = await tenantAccessor.CurrentTenant.Value;

            // This service was registered as singleton in tenant container.
            //  var someTenantService = container.GetService<SomeTenantService>();

            // The tenant shell to access context for the tenant - even if the tenant is null
            var tenantShellAccessor = context.RequestServices.GetRequiredService<ITenantShellAccessor<Tenant>>();
            var tenantShell = await tenantShellAccessor.CurrentTenantShell.Value;


            string tenantShellId = tenantShell == null ? "{NULL TENANT SHELL}" : tenantShell.Id.ToString();
            string tenantName = tenant == null ? "{NULL TENANT}" : tenant.Name;
            //  string injectedTenantName = someTenantService?.TenantName ?? "{NULL SERVICE}";

            // Accessing a content file.
            // string fileContent = someTenantService?.GetContentFile("/Info.txt");
            context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
            var result = new
            {
                TenantShellId = tenantShellId,
                TenantName = tenantName,
                //  TenantScopedServiceId = someTenantService?.Id,
                //  InjectedTenantName = injectedTenantName,
                //  TenantContentFile = fileContent
            };

            var jsonResult = JsonConvert.SerializeObject(result);
            await context.Response.WriteAsync(jsonResult, Encoding.UTF8);
            logger.LogDebug("App Run Finished..");
        }
    }



}

