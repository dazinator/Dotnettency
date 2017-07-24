# Dotnettency
Mutlitenancy library for dotnet applications.

Heavily inspired by [saaskit](https://github.com/saaskit/saaskit)

See the sample app for usage.

Let's you inject `ITenantAccessor<TTenant>` in order to lazily access the current tenant (which could be null).

Let's you inject `ITenantShellAccessor<TTenant>` in order to access context for the tenant, which is primarily used by:

- Extensions (such as Middleware, or Container) - which store things for the tenant in the `ITenantShellAccessor<TTenant>`'s concurrent property bag.
- Tenant Admin screens - if you need to "Restart" a tenant, then the idea is, you can resolve the `ITenantShellAccessor<TTenant>` and then use extension methods (provided by the dotnettency extensions such as Middleware pipeline, or Container) to allow you to control the state of the running tenant - for example to trigger rebuild of the tenant's container, or piepline on the next request.

TODO:

See issues.
