c# Dotnettency
Dotnettency is a library that provides features to enable Multi-Tenant applications using either:
  - ASP.NET Core
  - OWIN

| Branch  | Build Status | 
| ------------- | ------------- |
| Master  | [![Build status](https://ci.appveyor.com/api/projects/status/2xi1nts54u2hamv3/branch/master?svg=true)](https://ci.appveyor.com/project/dazinator/dotnettency/branch/master) | 
| Develop | [![Build status](https://ci.appveyor.com/api/projects/status/2xi1nts54u2hamv3/branch/develop?svg=true)](https://ci.appveyor.com/project/dazinator/dotnettency/branch/develop) | 

| Branch  | Dotnettency Core Library | AspNetCore | Owin | EF Core | Tenant File System |
| ------------- | ------------- | ----- | ----- | ----- | ---- |
| Master  | [![Dotnettency](https://img.shields.io/nuget/v/Dotnettency.svg)](https://www.nuget.org/packages/Dotnettency/) | [![AspNetCore](https://img.shields.io/nuget/v/Dotnettency.AspNetCore.svg)](https://www.nuget.org/packages/Dotnettency.AspNetCore/) | [![Owin](https://img.shields.io/nuget/v/Dotnettency.Owin.svg)](https://www.nuget.org/packages/Dotnettency.Owin/) | [![EF Core](https://img.shields.io/nuget/v/Dotnettency.EFCore.svg)](https://www.nuget.org/packages/Dotnettency.EFCore/) | [![Tenant FileSystem](https://img.shields.io/nuget/v/Dotnettency.TenantFileSystem.svg)](https://www.nuget.org/packages/Dotnettency.TenantFileSystem/) |
| Develop | [![Dotnettency](https://img.shields.io/nuget/vpre/Dotnettency.svg)](https://www.nuget.org/packages/Dotnettency/) | [![AspNetCore](https://img.shields.io/nuget/vpre/Dotnettency.AspNetCore.svg)](https://www.nuget.org/packages/Dotnettency.AspNetCore/) | [![Owin](https://img.shields.io/nuget/vpre/Dotnettency.Owin.svg)](https://www.nuget.org/packages/Dotnettency.Owin/) | [![EF Core](https://img.shields.io/nuget/vpre/Dotnettency.EFCore.svg)](https://www.nuget.org/packages/Dotnettency.EFCore/) | [![Tenant FileSystem](https://img.shields.io/nuget/vpre/Dotnettency.TenantFileSystem.svg)](https://www.nuget.org/packages/Dotnettency.TenantFileSystem/) |

| Branch | Autofac | StructureMap |
| ------------- | ------------- | ------------- |
| Master | [![Autofac](https://img.shields.io/nuget/v/Dotnettency.Container.Autofac.svg)](https://www.nuget.org/packages/Dotnettency.Container.Autofac/) | [![StructureMap](https://img.shields.io/nuget/v/Dotnettency.Container.StructureMap.svg)](https://www.nuget.org/packages/Dotnettency.Container.StructureMap/) |
| Develop |  [![Autofac](https://img.shields.io/nuget/vpre/Dotnettency.Container.Autofac.svg)](https://www.nuget.org/packages/Dotnettency.Container.Autofac/) | [![StructureMap](https://img.shields.io/nuget/vpre/Dotnettency.Container.StructureMap.svg)](https://www.nuget.org/packages/Dotnettency.Container.StructureMap/) |


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
