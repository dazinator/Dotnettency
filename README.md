# Dotnettency
Mutlitenancy library for dotnet applications.

| Branch  | Build Status | Dotnettency Core Library | Middleware | Container | StructureMap |
| ------------- | ------------- | ----- | ----- | ----- | ----- |
| Master  |[![Build status](https://ci.appveyor.com/api/projects/status/2xi1nts54u2hamv3/branch/master?svg=true)](https://ci.appveyor.com/project/dazinator/dotnettency/branch/master) | [![Dotnettency](https://img.shields.io/nuget/v/Dotnettency.svg)](https://www.nuget.org/packages/Dotnettency/) | [![MiddlewarePipeline](https://img.shields.io/nuget/v/Dotnettency.MiddlewarePipeline.svg)](https://www.nuget.org/packages/Dotnettency.MiddlewarePipeline/) | [![Container](https://img.shields.io/nuget/v/Dotnettency.Container.svg)](https://www.nuget.org/packages/Dotnettency.Container/) | [![StructureMap](https://img.shields.io/nuget/v/Dotnettency.Container.StructureMap.svg)](https://www.nuget.org/packages/Dotnettency.Container.StructureMap/) |
| Develop | [![Build status](https://ci.appveyor.com/api/projects/status/2xi1nts54u2hamv3/branch/develop?svg=true)](https://ci.appveyor.com/project/dazinator/dotnettency/branch/develop)  | [![Dotnettency](https://img.shields.io/nuget/vpre/Dotnettency.svg)](https://www.nuget.org/packages/Dotnettency/) | [![MiddlewarePipeline](https://img.shields.io/nuget/vpre/Dotnettency.MiddlewarePipeline.svg)](https://www.nuget.org/packages/Dotnettency.MiddlewarePipeline/) | [![Container](https://img.shields.io/nuget/vpre/Dotnettency.Container.svg)](https://www.nuget.org/packages/Dotnettency.Container/) |  [![StructureMap](https://img.shields.io/nuget/vpre/Dotnettency.Container.StructureMap.svg)](https://www.nuget.org/packages/Dotnettency.Container.StructureMap/) |



Heavily inspired by [saaskit](https://github.com/saaskit/saaskit)

See the sample app for a full display of all the current features which include:

- Tenant resolution
- Per Tenant Middleware Pipeline
- Per Tenant Containers
- Per Tenant HostingEnvironment

Tenant Resolution

One configured in `startup.cs` you can:

- Inject `TTenant` directly.
- Inject `ITenantAccessor<TTenant>` in order to lazily access the current tenant (which could be null).
- Inject `ITenantShellAccessor<TTenant>` in order to access context for the tenant, which is primarily used by:
  - Extensions (such as Middleware, or Container) - which store things for the tenant in the `ITenantShellAccessor<TTenant>`'s concurrent property bag.
  - Tenant Admin screens - if you need to "Restart" a tenant, then the idea is, you can resolve the `ITenantShellAccessor<TTenant>` and then use extension methods (provided by the dotnettency extensions such as Middleware pipeline, or Container) to allow you to control the state of the running tenant - for example to trigger rebuild of the tenant's container, or pipeline on the next request.


Notes: For details on what Per Tenant Hosting Environment does see the README on the sample.
