# THE END IS NIGH

## 28/09/2021: I have taken the decision to archive this repository and discontinue development. I have learned a lot with this project. I beleive that although this project does solve some problems, there are nicer ways that will yield cleaner code, and a cleaner solution architecture. I have begun work on a side project which will eventually replace this.

c# Dotnettency
Dotnettency is a library that provides features to enable Multi-Tenant applications using either:
  - ASP.NET Core
  - OWIN

| Branch  | AppVeyor | DevOps |
| ------------- | ------------- | --- |
| Master  | [![Build status](https://ci.appveyor.com/api/projects/status/2xi1nts54u2hamv3/branch/master?svg=true)](https://ci.appveyor.com/project/dazinator/dotnettency/branch/master) |  |
| Develop | [![Build status](https://ci.appveyor.com/api/projects/status/2xi1nts54u2hamv3/branch/develop?svg=true)](https://ci.appveyor.com/project/dazinator/dotnettency/branch/develop) | [![Build Status](https://dev.azure.com/darrelltunnell/Public%20Projects/_apis/build/status/dazinator.Dotnettency?branchName=develop)](https://dev.azure.com/darrelltunnell/Public%20Projects/_build/latest?definitionId=5&branchName=develop) |


| Package  | Stable | Pre-release |
| ------------- | --- | --- |
| Dotnettency Core  | [![Dotnettency](https://img.shields.io/nuget/v/Dotnettency.svg)](https://www.nuget.org/packages/Dotnettency/) | [![Dotnettency](https://img.shields.io/nuget/vpre/Dotnettency.svg)](https://www.nuget.org/packages/Dotnettency/) | 
| AspNetCore | [![AspNetCore](https://img.shields.io/nuget/v/Dotnettency.AspNetCore.svg)](https://www.nuget.org/packages/Dotnettency.AspNetCore/) | [![AspNetCore](https://img.shields.io/nuget/vpre/Dotnettency.AspNetCore.svg)](https://www.nuget.org/packages/Dotnettency.AspNetCore/) | 
| Owin |  [![Owin](https://img.shields.io/nuget/v/Dotnettency.Owin.svg)](https://www.nuget.org/packages/Dotnettency.Owin/)  |  [![Owin](https://img.shields.io/nuget/vpre/Dotnettency.Owin.svg)](https://www.nuget.org/packages/Dotnettency.Owin/) |
| EF Core |  [![EF Core](https://img.shields.io/nuget/v/Dotnettency.EFCore.svg)](https://www.nuget.org/packages/Dotnettency.EFCore/)  | [![EF Core](https://img.shields.io/nuget/vpre/Dotnettency.EFCore.svg)](https://www.nuget.org/packages/Dotnettency.EFCore/) |
| Tenant File System | [![Tenant FileSystem](https://img.shields.io/nuget/v/Dotnettency.TenantFileSystem.svg)](https://www.nuget.org/packages/Dotnettency.TenantFileSystem/) | [![Tenant FileSystem](https://img.shields.io/nuget/vpre/Dotnettency.TenantFileSystem.svg)](https://www.nuget.org/packages/Dotnettency.TenantFileSystem/) |
| Autofac  |  [![Autofac](https://img.shields.io/nuget/v/Dotnettency.Container.Autofac.svg)](https://www.nuget.org/packages/Dotnettency.Container.Autofac/) | [![Autofac](https://img.shields.io/nuget/vpre/Dotnettency.Container.Autofac.svg)](https://www.nuget.org/packages/Dotnettency.Container.Autofac/)  |
| StructureMap | [![StructureMap](https://img.shields.io/nuget/v/Dotnettency.Container.StructureMap.svg)](https://www.nuget.org/packages/Dotnettency.Container.StructureMap/) | [![StructureMap](https://img.shields.io/nuget/vpre/Dotnettency.Container.StructureMap.svg)](https://www.nuget.org/packages/Dotnettency.Container.StructureMap/) |
| Configuration | [![Dotnettency](https://img.shields.io/nuget/v/Dotnettency.Configuration.svg)](https://www.nuget.org/packages/Dotnettency.Configuration/) | [![Dotnettency](https://img.shields.io/nuget/vpre/Dotnettency.Configuration.svg)](https://www.nuget.org/packages/Dotnettency.Configuration/) | 

Inspired by [saaskit](https://github.com/saaskit/saaskit)

## Resources

 - Tutorial series here: http://darrelltunnell.net/tutorial/creating-modular-multi-tenant-asp-net-core-application-with-dotnettency/
 - Various samples here: https://github.com/dazinator/Dotnettency.Samples
 - More extensive [sample app for a full display of all the current features](https://github.com/dazinator/Dotnettency/tree/master/src/Dotnettency.Sample) or if you want to see MVC in action, checkout the [MVC sample](https://github.com/dazinator/Dotnettency/tree/develop/src/Sample.Mvc)
 
## Features

### Tenant resolution

You can define how you want to identify the current tenant, i.e using a url scheme, cookie, or any of your custom logic.
You can then access the current tenant through dependency injection in your app.

### Tenant Middleware Pipelines
In your web application (OWIN or ASP.NET Core), when the web server recieves a request, it typically runs it through a single "middleware pipeline".
`Dotnettency` allows you to have a lazily initialised "Tenant Middleware Pipeline" created for each distinct tenant. In the tenant specific middleware pipeline, you can choose to include middleware conditionally based on current tenant information.
For example, for one tenant, you may use Facebook Authentication middleware, where as for another you might not want that middleware enabled.

### Tenant Containers / Services
In ASP.NET Core applications (Dotnettency also allows you to achieve this in OWIN applications even though OWIN doesn't cater for this pattern out of the box), you configure a global set of services on startup for dependency injection purposes.
At the start of a request, ASP.NET Core middleware creates a scoped version of those services to satisfy that request.
`Dotnettency` goes a step further, by allowing you to register services for each specific tenant. Dotnettency middleware then 
provides an `IServiceProvider` scoped to the request for the current tenant. This means services that are typically injected into your classes during a request can now be tenant specific.
This is useful if, for example, you want one tenant to use a different `IPaymentProvider` etc from another based on tenant settings etc.

### Tenant File System
Notes: For more in depth details on what Per Tenant File System is, see the [README on the sample](https://github.com/dazinator/Dotnettency/tree/master/src/Dotnettency.Sample).

Allows you to configure an `IFileProvider` that returns files from a virtual directory build for the current tenant.
For example, tenant `foo` might want to access a file `/bar.txt` which exists for them, but when tenant `bar` tries to access `/bar.txt` it doesn't exist for them - because each tenant has it's own distinct virtual directory.
Tenant virtual directories can overlap by sharing access to common directories / files.

## Tenant Injection

Once configured in `startup.cs` you can resolve the current tenant in any one of the following ways:

- Inject `TTenant` directly (may block whilst resolving current tenant).
- Inject `Task<TTenant>` - Allows you to `await` the current `Tenant` (so non blocking). `Task<TTenant>` is convenient.
- Inject `ITenantAccessor<TTenant>`. This is similar to injecting `Task<Tenant>` in that it provides lazy access the current tenant in a non blocking way. For convenience it's now easier to just inject `Task<Tenant>` instead, unless you want a more descriptive API.

## Tenant Restart (New in v2.0.0)

You can `Restart` a tenant. This does not stop the web application, or interfere with other tenants.
When you trigger a `Restart` of a tenant, it means the current tenants `TenantShell` (and all state, such as Services, MiddlewarePipeline etc) are disposed of.
Once the Restart has finished, it means the next http request to that tenant will result in the tenant intialising again from scratch.
This is useful for example, if you register services or middleware based on some settings, and you want to allow the settings to be changed for the tenant and therefore services  middleware pipeline to be rebuilt based on latest config.
It is also useful if you have a plugin based architecture, and you want to allow tenants to install plugins whilst the system is running.

   - Tenant Container will be re-built (if you are usijng tenant services the method you use to register services for the current tenant will be re-rexecuted.)
   - Tenant Middleware Pipeline will be re-built (if you are using tenant middleware pipeline, it will be rebuilt - you'll have a chance to include additional middlewares etc.)

For sample usage, see the Sample.AspNetCore30.RazorPages sample in this solution, in partcular the Pages/Gicrosoft/Index.cshtml page.

Injext `ITenantShellRestarter<Tenant>` and invoke the `Restart()` method:

```
    public class IndexModel : PageModel
    {

        public bool IsRestarted { get; set; }

        public void OnGet()
        {

        }

        public async Task OnPost([FromServices]ITenantShellRestarter<Tenant> restarter)
        {
            await restarter.Restart();
            IsRestarted = true;
            this.Redirect("/");
        }
    }
```

and corresponding razor page:

```

@page
@using Sample.Pages.T1
@model IndexModel
@{
    ViewData["Title"] = "Home page";
}

<div class="text-center">
    <h1>Tenant Gicrosoft Razor Pages!</h1>

    <form method="post">
        @{
            if (!@Model.IsRestarted)
            {
                <button asp-page="Index">Restart Tenant</button>
            }
            else
            {
                <button disabled asp-page="Index">Restart Tenant</button>
                <p>Tenant has been restarted, the next request will result in Tenant Container being rebuilt, and tenant middleware pipeline being re-initialised.</p>
            }
        }

    </form>
</div>

```
## Tenant Shell Injection

The `TenantShell` stores the context for a Tenant, such as it's `Container` and it's `MiddlewarePipeline`.
It's stored in a cache, and is evicted if the tenant is Restarted.
You probably won't need to use it directly, but if you want you can do so.

  - Inject `ITenantShellAccessor<TTenant>` to access the TenantShell for the current tenant.
  - Extensions (such as Middleware, or Container) - store things for the tenant in it's concurrent property bag. You can get at these properties if you know the keys.
  - You can also register callbacks that will be invoked when the TenantShell is disposed of - this happens when the tenant is restarted for example.

  Another way to register code that will run when the tenant is restarted, is to use TenantServices - add a disposable singleton service the tenant's container.
  When the tenant is disposed of, it's container will be disposed of, and your disposable service will be disposed of - depending upon your needs this hook might suffice.

  ### Tenant IConfiguration (New in v2.1.0)

ASP.NET Core hosting model allows you to build an `IConfiguration` for your application settings.
Dotnettency takes this further, by allowing each tenant to have it's own `IConfiguration` lazily constructed when the tenant is initialised (first request to the tenant).
You can access the current tenant's `IConfiguration` by injecting `Task<IConfiguration>` into your Controllers. The snippet below shows how to configure tenant specific configuration, notice how it uses the current tenant's name to find the JSON file:

```
 .ConfigureTenantConfiguration((a) =>
                        {
                            var tenantConfig = new ConfigurationBuilder();
                            tenantConfig.AddJsonFile(Environment.ContentRootFileProvider, $"/appsettings.{a.Tenant?.Name}.json", true, true);
                            return tenantConfig;
                        })
						
```

You can now inject `Task<IConfiguration>` into your controllers etc, and `await` the result to obtain the tenants `IConfiguration`.
Note: if you inject `IConfiguration` rather than `Task<IConfiguration>` you will get the usual application wide `IConfiguration` like normal (currently).

You can access the Tenant's `IConfiguration' when building the Tenant's middleware pipeline, or Container - this is designed such that you could use tenant specific configuration to decide how to configure that tenants middleware or services.

## Tenant Shell Items

Tenant Shell Items are special kind of item that have a lifetime tied to the current tenant, and are stored in the tenant's `TenantShell`. 
They are:

1. Lazily initialised on first access, per tenant.
2. Stored in the tenant's `TenantShell` for the lifetime of that tenant.
3. If the tenant is restarted, the value is cleared from the tenant's shell, and lazily re-initialised on next access again.
4. ~~If your `TItem` implements `IDisposable` it will be disposed of when the it is removed from the `TenantShell` (typically on a tenant restart) accessed asynchronously.~~ Not currently implemented.
5. It will be registered for DI as `Task<TItem>` so you can inject it and then await the `Task` to get the value. The await is necessary as the value will
   be asynchronously created only on first access for that tenant. On subsequent accesses, the same cached task (already completed) is used to return the value immediately.

You can register a tenant shell item during dotnettency fluent configuration like:

```csharp

             services.AddMultiTenancy<Tenant>((builder) =>
             {
                 builder.IdentifyTenantsWithRequestAuthorityUri()
                         // .. shortened for brevity
                        .ConfigureTenantShellItem((tenantInfo) =>
                        {
                            return new ExampleShellItem { TenantName = tenantInfo.Tenant?.Name };
                        })

```

You can now access this through DI:

```

public class MyController
{
      public MyController(Task<ExampleShellItem> shellItem)
	  {
	  
	  }

}

```
Note: If you don't like injecting `Task<T>` you can also inject `ITenantShellItemAccessor<TTenant, TItem>` and use that to get access to the shell item

You can also access the shell item during most fluent configuration of a tenant, for example most fluent configuration methods expose a context object with a `GetShellItemAsync` extension method:

```
 var exampleShellItem = await context.GetShellItemAsync<ExampleShellItem>();

```


## Named Shell Items?

Suppose you want to register multiple of your Shell Item instances, with different names. You can use the `ConfigureNamedTenantShellItems` instead.

```
             services.AddMultiTenancy<Tenant>((builder) =>
             {
                 builder.IdentifyTenantsWithRequestAuthorityUri()
                         // .. shortened for brevity
                         .ConfigureNamedTenantShellItems<Tenant, ExampleShellItem>((b) =>
                         {
                             b.Add("red", (c) => new ExampleShellItem(c.Tenant?.Name ?? "NULL TENANT") { Colour = "red" });
                             b.Add("blue", (c) => new ExampleShellItem(c.Tenant?.Name ?? "NULL TENANT") { Colour = "blue" });
                         });


```

To access a named shell item through DI, rather than injecting `Task<T>`, you can inject `Func<string, `Task<T>`:


```csharp

public class MyController
{
      private readonly Func<string, Task<T>> _namedShellItemFactory
     
	  public MyController(Func<string, Task<T>> namedShellItemFactory)
	  {
	     _namedShellItemFactory = namedShellItemFactory;
	  }

	  public async Task<T> GetRedItem()
	  {
	      return await _namedShellItemFactory("red");
	  }

}

```      

Or during fluent configuration of the tenant, for exmaple whilst configuring the tenant's middleware pipeline or services:

```csharp 

 var redShellItem = await context.GetShellItemAsync<ExampleShellItem>("red");

```

## Tenant Mapping (New in v3.0)

There now exists a newer API to help you get up and running more quickly.
In most cases you just want to map some value during an incoming request (using some value current httpcontext like Request.Hostname etc.) to a unique
identifier, that can then be used to establish the correct context for the current tenant.

So this is the new way to configure dotnettency on startup (the old api's still exist and work):

```csharp
            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.AddAspNetCore()                      
                       .IdentifyFromHttpContext<int>((m) =>
                       {
                           m.MapRequestHost() // you can optionally use lambda here to select from any available value in httpcontext
                            .WithMapping((tenants) =>
                            {
                                tenants.Add(1, "*.foo.com", "*.foo.uk");
                                tenants.Add(2, "t2.bar.com", "t1.foo.uk");
                            })
                            .UsingDotNetGlobPatternMatching(); // add the Dotnettency.DotNetGlob package for this extension method.
                            .Initialise((key) =>
                            {
                                // e.g return Tenant info that you need for the mapped key.
                                if (key == 1)
                                {
                                    var tenant = new Tenant() { Id = key, Name = "Test Tenant" };
                                    return Task.FromResult(tenant);
                                }
                                return null; // we somehow mapped an invalid key - return null.
                            });                            
                      });                       
            });
```

There are various options available for these API's.

You can also put your mapping in a config file, rather than using `.WithMapping()`:

```
            ServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();
            services.AddMultiTenancy<Tenant>((builder) =>
            {
                builder.AddAspNetCore()                      
                       .IdentifyFromHttpContext<int>((m) =>
                       {
                           m.MapRequestHost()                            
                            .UsingDotNetGlobPatternMatching(); // add the Dotnettency.DotNetGlob package for this extension method.
                            .Initialise((key) =>
                            {
                                // e.g load tenant info for mapped key.
                                if (key == 1)
                                {
                                    var tenant = new Tenant() { Id = key, Name = "Test Tenant" };
                                    return Task.FromResult(tenant);
                                }
                                return Task.FromResult<Tenant>(null); // we must have mapped an invalid key.
                            });                            
                      });                       
            });

            // Let's put our mapping in configuration - then it will reload if we make config changes!
            IConfigurationSection configSection = Configuration.GetSection("Tenants");
            services.Configure<TenantMappingOptions<int>>(configSection);
```

Then in your `appsettings.json`:

```

{
  "Mappings": [
    {
      "Key": 1,
      "Patterns": [ "*.foo.com", "*.foo.org" ]
    },
    {
      "Key": 2,
      "Patterns": [ "*.bar.com" ]
    }
  ]
}

```

The call to `.UsingDotNetGlobPatternMatching();` allows you to use the glob pattern syntax specified here: https://github.com/dazinator/DotNet.Glob#patterns


### Conditionally enabled mappings

You might want to put in place a mapping that is conditionally enabled based on some application check / state.
For example, whilst your application is in "Setup Mode", you might want all requests mapped to a special -1 tenant key, which will indicate the system setup tenant shell.
Then you can initialise that shell just for the system setup experience / api's etc.

You can do this like so:


```
  
     var isSetupComplete = false;

     .IdentifyFromHttpContext<int>((m) =>
     {
         m.MapRequestHost()
         .WithMapping((a) =>
         {
             a.Add(-1, new string[] { "**" }, "IsSetupComplete", false);
             a.Add(1, new string[] { "*.foo.com", "*.foo.uk" });
         })
         .RegisterConditions((c) =>
         {
             c.Add("IsSetupComplete", (sp) =>
             {
                 return isSetupComplete; // function called to evaluate this condition when matching a request.
             });
         })          
         .Initialise((key) =>
         {
              if(key == -1)
              {
                var tenant = new Tenant() { Id = key, Name = "System Setup Tenant" };
			  }
              else
              {
                var tenant = new Tenant() { Id = key, Name = "Foo" };
                return Task.FromResult(tenant);
			  }             
          });
     });        

```

Of if you prefer config, remove the WithMapping() call, and place this in your appsettings.json instead:

```
{
  "Mappings": [
    {
      "Key": -1,
      "Patterns": [ "**" ],
      "Condition": {
        "Name": "IsSetupComplete",
        "RequiredValue": false
      }
    },
    {
      "Key": 1,
      "Patterns": [ "t1.foo.com", "*.foo.uk" ]      
    }
  ]
}


```

Now whilst `isSetupComplete` is `false` the first mapping will be enabled, which will match all requests, and map to a tenant key of -1.
When you initialise the tenant, like load its services, and middelware etc, you can only add services / middleware required for your system setup experience.
Once system setup is complete, your IsSsytemSetup condition function should start returning `true` which will then disable this first mapping. From that
point onwards, request mapping will then proceed to evaluate the rest of the mappings to resolve a reuqest to a tenant key - in this case `t1.foo.com` and any subdomain of `foo.uk` will resolve to tenant key 1.


### Catch-all mapping

You might want to ensure that you catch any requests not mapped to other tenants, with a catch all mapping that you can map to a special key to initialise a "default" or "welcome page" tenant.
You'd just ensure you have this mapping at the bottom of your mappings list. As long as no other mapping above it maps to another key, then this is essentially the fallback key.

```

{
  "Mappings": [
    {
      "Key": -2,
      "Patterns": [ "**" ]
    }
  ]
}

```

As you on-board new tenants, you'd update these mappings to add the url mapping for each tenant, above that catch-all mapping:

```
{
  "Mappings": [
    {
      "Key": 1,
      "Patterns": [ "*.foo.com" ]
    },
    {
      "Key": -2,
      "Patterns": [ "**" ]
    }
  ]
}

```

Now any request that comes in for a tenant not set up will be caught and mapped to -2 key, and you can use that when initialising that tenant to 
only add the services / middleware etc required to present your welcome page.


Having trouble updating the Json file? Yes System.Text.Json isn't currently the best - back to Newtonsoft?


## Notes

### Serilog

When configuring serilog, there is [an issue to be aware of](https://github.com/serilog/serilog-extensions-logging/issues/163)
Instead of calling `UseSerilog` on the HostBuilder, you need to do this:

```
  webhostBuilder.ConfigureLogging((logging) =>
                {
                    logging.ClearProviders();// clears microsoft providers registered by default like console.
                    logging.AddSerilog(Logger);
                });
```
