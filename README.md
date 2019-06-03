c# Dotnettency
Dotnettency is a library that provides features to enable Multi-Tenant applications using:
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

## Tenant Shell Injection

The `TenantShell` stores additional context for a Tenant, such as it's `Container` and it's `MiddlewarePipeline`.

- Inject `ITenantShellAccessor<TTenant>` in order to access context for the currnet tenant, which is primarily used by:
  - Extensions (such as Middleware, or Container) - which store things for the tenant in the `ITenantShellAccessor<TTenant>`'s concurrent property bag.
  - Tenant Admin screens - if you need to "Restart" a tenant, then the idea is, you can resolve the `ITenantShellAccessor<TTenant>` and then use extension methods (provided by the dotnettency extensions such as Middleware pipeline, or Container) to allow you to control the state of the running tenant - for example to trigger rebuild of the tenant's container, or pipeline on the next request.
   