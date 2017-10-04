c# Dotnettency
Mutlitenancy library for dotnet applications.

| Branch  | Build Status | Dotnettency Core Library | Middleware | Container | StructureMap |
| ------------- | ------------- | ----- | ----- | ----- | ----- |
| Master  |[![Build status](https://ci.appveyor.com/api/projects/status/2xi1nts54u2hamv3/branch/master?svg=true)](https://ci.appveyor.com/project/dazinator/dotnettency/branch/master) | [![Dotnettency](https://img.shields.io/nuget/v/Dotnettency.svg)](https://www.nuget.org/packages/Dotnettency/) | [![MiddlewarePipeline](https://img.shields.io/nuget/v/Dotnettency.MiddlewarePipeline.svg)](https://www.nuget.org/packages/Dotnettency.MiddlewarePipeline/) | [![Container](https://img.shields.io/nuget/v/Dotnettency.Container.svg)](https://www.nuget.org/packages/Dotnettency.Container/) | [![StructureMap](https://img.shields.io/nuget/v/Dotnettency.Container.StructureMap.svg)](https://www.nuget.org/packages/Dotnettency.Container.StructureMap/) |
| Develop | [![Build status](https://ci.appveyor.com/api/projects/status/2xi1nts54u2hamv3/branch/develop?svg=true)](https://ci.appveyor.com/project/dazinator/dotnettency/branch/develop)  | [![Dotnettency](https://img.shields.io/nuget/vpre/Dotnettency.svg)](https://www.nuget.org/packages/Dotnettency/) | [![MiddlewarePipeline](https://img.shields.io/nuget/vpre/Dotnettency.MiddlewarePipeline.svg)](https://www.nuget.org/packages/Dotnettency.MiddlewarePipeline/) | [![Container](https://img.shields.io/nuget/vpre/Dotnettency.Container.svg)](https://www.nuget.org/packages/Dotnettency.Container/) |  [![StructureMap](https://img.shields.io/nuget/vpre/Dotnettency.Container.StructureMap.svg)](https://www.nuget.org/packages/Dotnettency.Container.StructureMap/) |



Heavily inspired by [saaskit](https://github.com/saaskit/saaskit)

## Resources

 - Tutorial series here: http://darrelltunnell.net/tags/dotnettency/
 - Various samples here: https://github.com/dazinator/Dotnettency.Samples
 - More extensive [sample app for a full display of all the current features](https://github.com/dazinator/Dotnettency/tree/master/src/Dotnettency.Sample) or if you want to see MVC in action, checkout the [MVC sample](https://github.com/dazinator/Dotnettency/tree/develop/src/Sample.Mvc)
 
## Features

- Tenant resolution
- Per Tenant Middleware Pipeline
- Per Tenant Containers
- Per Tenant HostingEnvironment
- Modules (Shared and Routed)

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


Notes: For details on what Per Tenant Hosting Environment does see the [README on the sample](https://github.com/dazinator/Dotnettency/tree/master/src/Dotnettency.Sample).
